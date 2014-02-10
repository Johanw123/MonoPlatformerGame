


using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Squared.Tiled;
using Lidgren.Network;

namespace MonoPlatformerGame
{
    public enum GameMode
    {
        //You have a cerain time to finish the course and the best time wins (track-mania style).
		//several tries, dying and reaching goal will put player back at start to try again
        TimeTrial,

        //First to the goal is the winner. Also if all dies, next map will be loaded
		//A fast pased game with high tempo
        Race,

        //Screen scrolls the same for all players. One life only, survivers that reach the goal are rewarded with a point.
		//Most point at end of certain ammount of levels will win.
        //(Coop/Versus?)
        Survival,
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Level level;
        Log log;
        private GameplayNetComponent gameplayNetComponent;
        bool isHost = true;
        
        
        Texture2D PauseTexture;

    	

        public Game1(String[] args)
        {
			_graphics = new GraphicsDeviceManager(this)
            {
				IsFullScreen = false
               //PreferredBackBufferHeight = 720,
                //PreferredBackBufferWidth = 1280
                
			};

            if (args.GetLength(0) > 0)
            {
                if (args[0] == "Host")
                    isHost = true;
            }
        }

		protected override void Initialize ()
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
		protected override void BeginRun ()
		{
			base.BeginRun ();
		}
        void gameplayNetComponent_ChangeLevelEvent(string levelName)
        {
            level.LoadLevel(levelName);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            Content.RootDirectory = "Content";
            ResourceManager.Initialize(Content, _graphics.GraphicsDevice);
            ParticleSystem.Init();
            ParticleSystem.AddEmitter("blood", new CircleEmitter());
            log = new JapeLogMono(this);
            Log.Init(log);
            PauseTexture = new Texture2D(GraphicsDevice, 1, 1);
            PauseTexture.SetData(new Color[] { Color.White });
            EntityManager.Init(GraphicsDevice);
            level = new Level(Content);
            //level.LoadLevel("Level10.tmx");
            
            JapeLog.WriteLine("IsHost: " + isHost);

            NetManager.Init(isHost);

            if (isHost)
                gameplayNetComponent = new HostGameplayNetComponent();
            else
                gameplayNetComponent = new ClientGameplayNetComponent();

            gameplayNetComponent.ChangeLevelEvent += gameplayNetComponent_ChangeLevelEvent;

            NetManager.AddComponent(gameplayNetComponent);
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            NetManager.Listen();
            Input.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
			if ((GamePad.GetState (PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Input.IsKeyPressed (Keys.S)) && !NetManager.GameStarted)
			{
				if (!level.LevelLoaded)
					level.LoadLevel ("Level10.tmx");

				NetManager.StartGame (level.LevelName);
			}
            if (!NetManager.GameStarted)
                return;
			if (!level.LevelLoaded)
				return;
            
            EntityManager.Update(deltaTime);
            ParticleSystem.Update(deltaTime);
            EntityManager.Collisions();
            EntityManager.UpdateCamera();
            gameplayNetComponent.Update();

            base.Update(gameTime);
        }

        protected override void Draw (GameTime gameTime)
		{
			// GraphicsDevice.Clear(Color.FromNonPremultiplied(51, 51, 51, 255));
			GraphicsDevice.Clear (Color.Gray);
			if (level.LevelLoaded)
			{
				EntityManager.Draw (_spriteBatch);
				ParticleSystem.Draw (_spriteBatch);
			}
				if (!NetManager.GameStarted) {
                
					_spriteBatch.Begin ();
					_spriteBatch.Draw (PauseTexture, new Rectangle (0, 0, 1280, 720), Color.Black * 0.5f);
					_spriteBatch.DrawString (ResourceManager.GetFont ("Verdana"), "Waiting for game to start", new Vector2 (_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), Color.White);
					_spriteBatch.End ();
				}
				log.Draw ();

            base.Draw(gameTime);
        }
    }

}
