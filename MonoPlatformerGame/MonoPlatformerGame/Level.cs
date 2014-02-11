using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Squared.Tiled;
using Microsoft.Xna.Framework.Graphics;

namespace MonoPlatformerGame
{
    public class Level
    {
        ContentManager mContent;
        Map mCurrentMap;
        public Map Map { get { return mCurrentMap; } }
		public bool LevelLoaded { get; set; }
		public string LevelName { get; set; }


        public Level(ContentManager content)
        {
            mContent = content;
        }

        public void LoadLevel(string level)
        {
            string path = System.IO.Path.Combine(mContent.RootDirectory, level);

            if (File.Exists(path))
            {
                EntityManager.ClearAll();
                mCurrentMap = Map.Load(path, mContent);
                Runtime.LevelHeight = mCurrentMap.Height * mCurrentMap.TileHeight;
                Runtime.LevelWidth = mCurrentMap.Width * mCurrentMap.TileWidth;
                Runtime.TileSize = mCurrentMap.TileWidth;

                LoadCurrentLevel();
				ParseMapProperties ();
				LevelName = level;
				LevelLoaded = true;
                NetManager.CurrentLevelName = LevelName;
            }
        }

		public static bool MapExist(string level)
		{
			//string path = System.IO.Path.Combine(mContent.RootDirectory, level);
			string path = System.IO.Path.Combine ("Content/", level);

			if (File.Exists (path))
			{
				return true;
			}
			return false;
		}

		public void UnloadLevel()
		{
			LevelLoaded = false;
		}

		private void ParseMapProperties()
		{
			ModManager.SetupMods(this); 
		}

        public void LoadCurrentLevel()
        {
            loadMap(mCurrentMap);
            EntityManager.SetupBounds(mCurrentMap);
        }


        //Private functions
		private Spike CreateSpike (int x, int y, string texture)
		{
			SortedList<string, string> settings = new SortedList<string, string>();
			settings.Add("Texture", texture);
            return new Spike(x * Runtime.TileSize, y * Runtime.TileSize, settings);
		}

        private void loadMap(Map map)
        {
            foreach (var layer in map.Layers)
            {
                switch (layer.Key)
                {
                    case "Blocks":
                        int x = -1;
                        int y = 0;
                        foreach (var tile in map.Layers["Blocks"].Tiles)
                        {
                            ++x;
                            if (x >= map.Layers["Blocks"].Width)
                            {
                                x = 0;
                                ++y;
                            }

							switch (tile) 
							{
								case 1:
									Block block = new Block(x * Runtime.TileSize, y * Runtime.TileSize);
	                            	EntityManager.AddStaticEntity(x, y, block);
								break;
								case 2:
									Finish finish = new Finish(x * Runtime.TileSize, y * Runtime.TileSize);
		                            EntityManager.AddStaticEntity(x, y, finish);
								break;
								case 3:
									EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "SmallSpike"));
								break;
								case 4:
	                               	EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "SmallSpikeLeft"));
								break;
								case 5:
									EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "SmallSpikeRight"));
								break;
								case 6:
									EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "SmallSpikeTop"));
								break;
								case 7:
								    EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "Spike"));
								break;
								case 8:
								    EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "SpikeLeft"));
								break;
								case 9:
								    EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "SpikeRight"));
								break;
								case 10:
								    EntityManager.AddStaticEntity(x, y, CreateSpike(x, y, "SpikeTop"));
								break;
								case 11:
									//TODO
	                                //Player is here for now
	                                Player p = new Player(x * Runtime.TileSize, y * Runtime.TileSize);
	                                EntityManager.AddDynamicEntity(p);

	                                Start start = new Start(x * Runtime.TileSize, y * Runtime.TileSize);
	                                EntityManager.AddStaticEntity(x, y, start);
								break;
							}
							
							
					}
					break;
                }
            }

            //objects
            foreach (var objectGroup in map.ObjectGroups)
            {
                switch (objectGroup.Key)
                {
                    case "Objects":

                        foreach (var aObject in objectGroup.Value.Objects)
                        {
                            switch (aObject.Name)
                            {
                                case "Start":

                                    //TODO
                                    //new Start();
                                    

                                    break;
                                case "Finish":
                                    //TODO
                                    //new Finish();
                                    
                                    break;
                                case "MovingPlatform":
                                    //TODO
                                    //new Finish();
                                    MovingPlatform platform = new MovingPlatform(aObject.X, aObject.Y, aObject.Width, 
                                        float.Parse(aObject.Properties["startOffset"]),
                                        float.Parse(aObject.Properties["speed"])
                                        );
                                    EntityManager.AddDynamicEntity(platform);
                                    break;
                            }

                            
                        }
                        break;
                   
                }
            }
        }


    }
}
