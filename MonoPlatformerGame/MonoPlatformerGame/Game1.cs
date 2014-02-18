


using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Squared.Tiled;
using Lidgren.Network;
using System.Diagnostics;

namespace MonoPlatformerGame
{
   

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Level level;
        Log log;
        private GameplayNetComponent gameplayNetComponent;
		private ChatNetComponent chatNetComponent;
        Texture2D PauseTexture;
        private Stopwatch changingLevelTimer = new Stopwatch();
        //private bool changingLevel = false;
        private double changeLevel = -1;

        public Game1()
        {
			_graphics = new GraphicsDeviceManager(this);

			_graphics.IsFullScreen = false;
#if WINDOWS
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.PreferredBackBufferWidth = 1280;
#endif
        }

		protected override void Initialize()
		{
			base.Initialize();

			//_graphics.PreferredBackBufferHeight = 720;
			//_graphics.PreferredBackBufferWidth = 1280;
			//_graphics.IsFullScreen = false;
			//_graphics = new GraphicsDeviceManager(this)
			//{
				//PreferredBackBufferHeight = 720,
				//PreferredBackBufferWidth = 1280,
			//	IsFullScreen = true
			//};
		}

		protected override void BeginRun()
		{
			base.BeginRun();
		}

        void gameplayNetComponent_ChangeLevelEvent(string levelName, double delayTime)
        {
            EntityManager.ResetPlayer();
            level.UnloadLevel();
            level.LoadLevel(levelName);
			if(delayTime > 0)
			{
				changeLevel = delayTime;
				EntityManager.GetPlayer().IsDisabled = true;
			}
			else
			{
				NetManager.StartGame();
			}
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            Content.RootDirectory = "Content";
			Input.Init();
            ResourceManager.Initialize(Content, _graphics.GraphicsDevice);
            ParticleSystem.Init();
            ParticleSystem.AddEmitter("blood", new CircleEmitter());
            log = new JapeLogMono(this);
            Log.Init(log);
            PauseTexture = new Texture2D(GraphicsDevice, 1, 1);
            PauseTexture.SetData(new Color[] { Color.White });
            EntityManager.Init(GraphicsDevice);
            level = new Level(Content);
           
            NetManager.Init(false);

            gameplayNetComponent = new ClientGameplayNetComponent();

            gameplayNetComponent.ChangeLevelEvent += gameplayNetComponent_ChangeLevelEvent;
            //gameplayNetComponent.ChangeLevelPrepEvent += gameplayNetComponent_ChangeLevelPrepEvent;

            NetManager.AddComponent(gameplayNetComponent);
			NetManager.AddComponent(chatNetComponent = new ChatNetComponent());
        }

        void gameplayNetComponent_ChangeLevelPrepEvent(double delayTime)
        {
            changingLevelTimer.Reset();
            changingLevelTimer.Start();
            //changingLevel = true;
            changeLevel = delayTime;
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            NetManager.Listen();
            Input.Update();
			chatNetComponent.Update();

			if(Input.IsKeyDown("llol"))
				Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            //if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Input.IsKeyPressed(Keys.S)) && !NetManager.GameStarted)
            //{
            //    if (!level.LevelLoaded)
            //        level.LoadLevel ("Level10.tmx");

            //    NetManager.StartGame();
            //}
            if (!NetManager.GameStarted)
                return;
			if (!Runtime.CurrentLevel.Loaded)
				return;


			if(changeLevel < 0)
			{
				changeLevel = 0;
				EntityManager.GetPlayer().IsDisabled = false;
				NetManager.StartGame();
			}
			else
			{

				EntityManager.Update(deltaTime);
				ParticleSystem.Update(deltaTime);
				EntityManager.Collisions();
				EntityManager.UpdateCamera();
				gameplayNetComponent.Update();

				// if(changeLevel != -1)
				//  {
				//if (changingLevelTimer.ElapsedMilliseconds >= changeLevel * 1000)
				//{
				//    changingLevelTimer.Reset();
				//    changeLevel = -1;
				//}
			}
			if(changeLevel != 0)
				changeLevel -= deltaTime;
            //}

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
		{
			// GraphicsDevice.Clear(Color.FromNonPremultiplied(51, 51, 51, 255));
			GraphicsDevice.Clear(Color.Gray);

			if (Runtime.CurrentLevel.Loaded)
			{
				EntityManager.Draw(_spriteBatch);
				ParticleSystem.Draw(_spriteBatch);

			}
			if (!NetManager.GameStarted) 
            {
				_spriteBatch.Begin();
				_spriteBatch.Draw(PauseTexture, new Rectangle (0, 0, 1280, 720), Color.Black * 0.5f);
				_spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), "Waiting for game to start", new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), Color.White);
				_spriteBatch.End();
			}

			DrawLoadingScreen();

				log.Draw();
			chatNetComponent.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
		private void DrawLoadingScreen()
		{
			if (changeLevel > 0)
			{
				_spriteBatch.Begin();
				_spriteBatch.Draw(PauseTexture, new Rectangle(0, 0, 1280, 720), Color.Black * 0.5f);
				_spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), "Next Level Starts In: " + changeLevel, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 10), Color.White);
				DrawModsStrings();
				_spriteBatch.End();
			}
		}

		private void DrawModsStrings()
		{
			Vector2 position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 10);

			if(ModManager.CantStop)
			{
				_spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), "Cant Stop", position + new Vector2(0,40), Color.White);
			}
			if(ModManager.JumpMania)
			{
				_spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), "Jump Mania", position+ new Vector2(0,80), Color.White);
			}
			if(ModManager.DoubleJump >= 1)
			{
				_spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), "Double Jump: " + ModManager.DoubleJump, position+ new Vector2(0,120), Color.White);
			}
			if(ModManager.Gravity != 3000)
			{
				_spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), "Alternate Gravity", position+ new Vector2(0,160), Color.White);
			}
		}
    }
}
