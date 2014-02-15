using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MonoPlatformerGame
{
    public class Player : Entity
    {
        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;
        private int currentNumberOfJumps = 0;
        private bool onPlatform = false;
        private float platformX;
        private Vector2 platformVel;
        protected Stopwatch mElapsedTimer;

        public bool IsDisabled
        {
            get;
            set;
        }

        public Stopwatch ElapsedTimer
        {
            get { return mElapsedTimer; }
        }
        
        public Player(float x, float y, SortedList<string,string> settings = null)
            : base(x, y, settings)
        {
            mTexture = ResourceManager.GetTexture("Player - Copy");
            mElapsedTimer = new Stopwatch();
            Facing = eFacing.Right;
            UpdateBoundingBox();
        }

        public override void Update(float deltaTime)
        {
            HandleInput(deltaTime);
            ApplyPhysics(deltaTime);
            UpdateBoundingBox();

            if (mPosition.Y > Runtime.LevelHeight)
                EntityManager.ResetPlayer();

            if (isOnGround)
                currentNumberOfJumps = 0;
            
            isOnGround = false;
            onPlatform = false;
        }

        public override string GetName()
        {
            return "Player";
        }

        bool isOnGround = false;
        float previousBottom;

        public override void HandleCollide(Entity other)
        {
            switch (other.GetName())
            {
                case "Block":
                    HandleBlockCollision(other);
                    break;
                case "MovingPlatform":
                    isOnGround = true;
                    onPlatform = true;
                    platformX = other.X;
                    platformVel = other.Velocity;

                    HandleBlockCollision(other);


                    break;
                case "Finish":
                    //EntityManager.PlayerReachedFinish(0, mElapsedTimer.Elapsed);
                    EntityManager.LocalPlayerFinish(mElapsedTimer.Elapsed);
                    break;

				case "Spike":
					PlayerDied();
                    break;
            }
        }
		private void PlayerDied()
		{
			switch(Level.CurrentGameMode)
			{
				case GameMode.Race:
					NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                                                (int)DataType.PlayerDied

                                                );

                    IsDisabled = true;
					break;
				case GameMode.Survival:

					break;
				case GameMode.TimeTrial:
					EntityManager.ResetPlayer();
					break;
			}

            ParticleSystem.FireEmitterAt("blood", Position);
		}

        private void ApplyPhysics(float deltaTime)
        {
            if (IsDisabled)
                return;

            if (onPlatform)
            {
                X += platformVel.X *deltaTime;
            }
  
            Vector2 previousPosition = Position;

            mVelocity.X += mAccel.X * Constants.MOVE_ACC * deltaTime;

            DoJump(deltaTime);
            mVelocity.Y += MathHelper.Clamp(ModManager.Gravity, 2000, 50000) * deltaTime;

            mVelocity.Y = MathHelper.Clamp(mVelocity.Y, -Constants.MAX_FALL_SPEED, Constants.MAX_FALL_SPEED);
            //mVelocity.Y = -mVelocity.Y;
            mVelocity.X *= Constants.AIR_FRICTION;

            if (ModManager.CantStop)
            {
                mVelocity.X = Constants.MAX_MOVE_SPEED * mAccel.X;
            }

            mVelocity.X = MathHelper.Clamp(mVelocity.X, -Constants.MAX_MOVE_SPEED, Constants.MAX_MOVE_SPEED);

            Position += mVelocity * deltaTime;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            if (Position.X == previousPosition.X)
                mVelocity.X = 0;

            if (Position.Y == previousPosition.Y)
                mVelocity.Y = 0;
        }
        
        private void HandleInput(float deltaTime)
        {
            if(!ModManager.CantStop)
                mAccel.X = 0.0f;

            if (Input.IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < -0.5f)
            {
                mAccel.X = -1.0f;
                Facing = eFacing.Left;
            }
            else if (Input.IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0.5f)
            {
                mAccel.X = 1.0f;
                Facing = eFacing.Right;
            }
            else
                mVelocity.X *= 0.7f;

            if (Input.IsKeyDown(Keys.Space))
            {
                Constants.MAX_MOVE_SPEED = 800;
            }
            else
                Constants.MAX_MOVE_SPEED = 450;

            if (Input.IsKeyPressed(Keys.R))
                EntityManager.ResetPlayer();

            isJumping = Input.IsKeyDown(Keys.Up) || Input.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed 
               || ModManager.JumpMania;
        }

        private void DoJump(float deltaTime)
        {
            if (isJumping)
            {
                if ((!wasJumping && isOnGround) || jumpTime > 0.0f)
                {
                    jumpTime += deltaTime;
                }

                if (jumpTime > 0.0f && jumpTime <= Constants.MAX_JUMP_TIME)
                {
                    mVelocity.Y = Constants.JUMP_LAUNCH_VELOCITY * (1.0f - (float)Math.Pow(jumpTime / Constants.MAX_JUMP_TIME, Constants.JUMP_CONTROL_POWER));
                }
                else
                {
                    if (/*mVelocity.Y < -Constants.MAX_FALL_SPEED * 0.5f && */!wasJumping && currentNumberOfJumps < ModManager.DoubleJump)
                    {
                        mVelocity.Y = Constants.JUMP_LAUNCH_VELOCITY * (0.5f - (float)Math.Pow(jumpTime / Constants.MAX_JUMP_TIME, Constants.JUMP_CONTROL_POWER));
                        jumpTime += deltaTime;
                        currentNumberOfJumps++;
                    }
                    else
                    {
                        jumpTime = 0.0f;
                    }
                }
            }
            else
            {
                jumpTime = 0.0f;
            }
            if(!ModManager.JumpMania)
                wasJumping = isJumping;

            //return velocityY;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsDisabled)
                return;

            Vector2 origin = new Vector2(mTexture.Width / 2, mTexture.Height / 2);
            origin = Vector2.Zero;

            spriteBatch.Draw(mTexture, mPosition, null, Color, 0f, origin, Scale, SpriteEffects, 0);
            spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, null, Color, 0f, origin, SpriteEffects.None, 0);
           
            //spriteBatch.Draw(ResourceManager.GetTexture("BoundingBox"), BoundingBox, Color.White); //Draw Bounding Box
        }

        //private functions
        private void HandleBlockCollision(Entity other)
        {
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(BoundingBox, other.BoundingBox);

            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                // Resolve the collision along the shallow axis.
                if (absDepthY < absDepthX)
                {
                    // Ignore platforms, unless we are on the ground.
                    if (true)
                    {
                        //tillräkligt mycket takträff gör att man börjar falla ner
                        if (depth.Y > 3)
                        {
                            jumpTime = 0;
                            mVelocity.Y = 0;
                        }
                        else if (depth.Y < -5)
                        {
                            isOnGround = true;
                            //mVelocity.Y = 500;
                        }

                        // Resolve the collision along the Y axis.
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                        UpdateBoundingBox();
                    }
                }
                else
                {
                    // Resolve the collision along the X axis.
                    Position = new Vector2(Position.X + depth.X, Position.Y);

                    UpdateBoundingBox();
                }
            }

            previousBottom = other.BoundingBox.Bottom;
        }
    }
}
