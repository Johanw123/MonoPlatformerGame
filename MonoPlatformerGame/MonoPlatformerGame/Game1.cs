


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
        TimeTrial,

        //First to the goal is the winner.
        Race,

        //Screen scrolls the same for all players. One life only, survivers that reach the goal are rewarded.
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
                //PreferredBackBufferHeight = 720,
                //PreferredBackBufferWidth = 1280,
                IsFullScreen = false
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
			//_graphics.PreferredBackBufferWidth = 1280
			_graphics.IsFullScreen = false;
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
            level.LoadLevel("Level10.tmx");
            
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
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Input.IsKeyPressed(Keys.S)) && !NetManager.GameStarted)
                NetManager.StartGame();


            if (!NetManager.GameStarted)
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
				EntityManager.Draw (_spriteBatch);
				ParticleSystem.Draw (_spriteBatch);

				if (!NetManager.GameStarted) {
                
					_spriteBatch.Begin ();
					_spriteBatch.Draw (PauseTexture, new Rectangle (0, 0, 1280, 720), Color.Black * 0.5f);
					_spriteBatch.End ();
				}
				log.Draw ();

            base.Draw(gameTime);
        }
    }

}