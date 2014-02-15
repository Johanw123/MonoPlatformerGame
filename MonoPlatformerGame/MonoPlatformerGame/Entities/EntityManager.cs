using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoPlatformerGame
{
    public static class EntityManager
    {
        static MatrixSpacePartition<Entity> matrix;
        static List<Entity> staticEntities;
        static List<Entity> dynamicEntities;
        static List<NetPlayer> netPlayers;
        public static RectangleTree<Entity> rectangleTree;
        static Player player;
        static Camera camera;
        private static Vector2 startPosition;

        public static List<Entity> DynamicEntities
        {
            get { return dynamicEntities; }
        }
        public static List<NetPlayer> NetPlayers
        {
            get { return netPlayers; }
        }
        public static Player GetPlayer()
        {
            return player;
        }
        //Public function

        public static void ResetPlayer()
        {
            if (player != null)
            {
                //ParticleSystem.FireEmitterAt("blood", player.Position);
                player.Velocity = new Vector2(0, 0);
                player.Position = startPosition;
                player.UpdateBoundingBox();
                player.ElapsedTimer.Restart();
                player.IsDisabled = false;
            }
        }

        public static void LocalPlayerFinish(TimeSpan time)
        {

            EntityManager.ResetPlayer();

            NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                          (int)DataType.BroadcastMessage,
                          (int)DataType.ChatMessage,
                          "Player reached finish: " + time.ToString()
                          );


            NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                        (int)DataType.PlayerFinish,
                        NetManager.RemoteUID,
                        time.TotalMilliseconds
                        );

            NetManager.PlayerReachedFinish(0, (int)time.TotalMilliseconds);
        }
        

        public static void Init(GraphicsDevice graphics)
        {
            matrix = new MatrixSpacePartition<Entity>();
            dynamicEntities = new List<Entity>();
            staticEntities = new List<Entity>();
            netPlayers = new List<NetPlayer>();
            camera = new Camera(graphics.Viewport);        
        }

        public static void SetupBounds(Squared.Tiled.Map map)
        {
            int width = (map.Width + 2) * map.TileWidth;
            int height = (map.Height + 2) * map.TileHeight;

            rectangleTree = new RectangleTree<Entity>(lol, new Rectangle(-Runtime.TileSize, -Runtime.TileSize, width, height));

            foreach (Entity entity in dynamicEntities)
            {
                rectangleTree.Add(entity);
            }
        }

        private static Rectangle lol(Entity e)
        {
            return e.BoundingBox;
        }

        private static void ParticleCollisions()
        {
            foreach (Emitter emitter in ParticleSystem.GetAllEmitters())
            {
                foreach (Entity entityA in emitter.GetParticleList().ToArray())
                {
                    GetStaticCollisions(entityA);
                }
            }
            
        }
        private static void GetStaticCollisions(Entity entityA)
        {
            foreach (Entity entityB in GetCloseStatics(entityA))
            {
                if (entityB != null)
                {
                    if (entityA.BoundingBox.Intersects(entityB.BoundingBox))
                    {
                        if (UsePixelCollisions(entityA) || UsePixelCollisions(entityB))
                        {
                            Color[] bitsA = new Color[entityA.Texture.Width * entityA.Texture.Height];
                            entityA.Texture.GetData<Color>(bitsA);

                            Color[] bitsB = new Color[entityB.Texture.Width * entityB.Texture.Height];
                            entityB.Texture.GetData<Color>(bitsB);

                            if (IntersectPixels(entityA.BoundingBox, bitsA, entityB.BoundingBox, bitsB))
                            {
                                entityA.HandleCollide(entityB);
                                entityB.HandleCollide(entityA);
                            }
                        }
                        else
                        {
                            entityA.HandleCollide(entityB);
                            entityB.HandleCollide(entityA);
                        }
                    }
                }
            }
        }

        private static void GetStaticCollisionsSorted(Entity entityA)
        {
            SortedList<float, Entity> collisionsByDepth = new SortedList<float, Entity>();

            foreach (Entity entityB in GetCloseStatics(entityA))
            {
                if (entityB != null)
                {
                    if (entityA.BoundingBox.Intersects(entityB.BoundingBox))
                    {
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(entityA.BoundingBox, entityB.BoundingBox);

                        float depthSum = Math.Abs(depth.X) + Math.Abs(depth.Y);
                        try
                        {
                            if (UsePixelCollisions(entityA) || UsePixelCollisions(entityB))
                            {
                                Color[] bitsA = new Color[entityA.Texture.Width * entityA.Texture.Height];
                                entityA.Texture.GetData<Color>(bitsA);

                                Color[] bitsB = new Color[entityB.Texture.Width * entityB.Texture.Height];
                                entityB.Texture.GetData<Color>(bitsB);

                                if (IntersectPixels(entityA.BoundingBox, bitsA, entityB.BoundingBox, bitsB))
                                    collisionsByDepth.Add(depthSum, entityB);
                            }
                            else
                                collisionsByDepth.Add(depthSum, entityB);
                        }
                        catch (Exception) { }
                    }
                }
            }
            //resolve
            for (int i = collisionsByDepth.Count - 1; i >= 0; --i)
            {
                Entity entityB = collisionsByDepth.Values[i];
                entityA.HandleCollide(entityB);
                entityB.HandleCollide(entityA);
            }
        }


        public static void Collisions()
        {
            Entity[] bigList = GetBigList();
            
            foreach (Entity entityA in bigList)
            {
                if (entityA.IsDead)
                    continue;

                foreach (Entity entityB in GetSmallList(entityA))
                {
                    if (entityB.IsDead)
                        continue;
                    
                    if (entityA.GetName() != entityB.GetName() && entityA.BoundingBox.Intersects(entityB.BoundingBox))
                    {
                        entityA.HandleCollide(entityB);
                        entityB.HandleCollide(entityA);
                    }
                }

                GetStaticCollisionsSorted(entityA);
            }

            ParticleCollisions();
        }

        private static bool UsePixelCollisions(Entity entity)
        {
            switch (entity.GetName())
            {
                case "Particle":
                case "Enemy":
                case "Player":
                    return true;

                default:
                    return false;
            }
        }

        private static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
        
        public static void ClearAll()
        {
            player = null;
            staticEntities.Clear();
            dynamicEntities.Clear();
            matrix = new MatrixSpacePartition<Entity>();
            //foreach (var item in GetBigList())
            //{
            //    rectangleTree.Remove(item);
            //}
            rectangleTree = null;
        }

        public static void Update(float deltaTime)
        {
            Entity[] bigList = GetBigList();

            foreach (Entity entity in bigList)
            {
                entity.Update(deltaTime);
                rectangleTree.UpdatePosition(entity);
            }
            foreach (NetPlayer netPlayer in netPlayers)
            {
                foreach (var client in NetManager.connectedClients)
                {
                    if (client.Value.UID == netPlayer.UID)
                    {
                        netPlayer.X = client.Value.X;
                        netPlayer.Y = client.Value.Y;
                    }
                }
            }
            //JapeLog.WriteLine(bigList.Count());
            RemoveDeadEntities(bigList);
            RemoveDisconnectedNetPlayers(netPlayers.ToArray());
        }

        private static void RemoveDeadEntities(Entity[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].IsDead)
                {
                    EntityManager.rectangleTree.Remove(list[i]);
                    dynamicEntities.Remove(list[i]);
                }
            }

        }

        private static void RemoveDisconnectedNetPlayers(NetPlayer[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Disconnected)
                {
                    netPlayers.Remove(list[i]);
                    NetManager.connectedClients.Remove(list[i].UID);
                }
            }

        }

        public static void UpdateCamera()
        {
            camera.Update();
        }
        public static Matrix GetCameraMatrix()
        {
            return camera.GetViewMatrix(new Vector2(1f));
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.GetViewMatrix(new Vector2(1f)));

            foreach (Entity entity in staticEntities)
            {
                entity.Draw(spriteBatch);
            }

            foreach (Entity entity in GetBigList())
            {
                entity.Draw(spriteBatch);
            }

            foreach (NetPlayer entity in netPlayers)
            {
                entity.Draw(spriteBatch);
                spriteBatch.DrawString(ResourceManager.GetFont("japelog"), NetManager.GetClient(entity.UID).Name, entity.Position + new Vector2(0, -20), Color.YellowGreen);
            }

            spriteBatch.End();
        }

        public static void AddStaticEntity(int x, int y, Entity e)
        {
            staticEntities.Add(e);
            matrix.Set(new Point(x,y), e);

            if (e.GetName() == "Start")
            {
                startPosition = e.Position;
            }
        }
        public static void AddNetPlayer(NetPlayer player)
        {
            netPlayers.Add(player);
        }
		public static void RemoveNetPlayer(int UID)
		{
            foreach (var item in netPlayers)
            {
                if (item.UID == UID)
                {
                    item.Disconnected = true;
                }
            }
		}
        public static void AddDynamicEntity(Entity e)
        {
            dynamicEntities.Add(e);
            if(rectangleTree != null)
                rectangleTree.Add(e);

            //TODO
            //FIxa snyggare kanske
            if (e.GetName() == "Player")
            {
                player = (Player)e;
                camera.SetFollow(player);
            }
            

        }

        //Private functions
        private static bool deadEntity(Entity entity)
        {
            return entity.IsDead;
        }

        private static Entity[] GetBigList()
        {
            Rectangle playerBigArea = player.BoundingBox;
            playerBigArea.Inflate(1000000, 1000000);
            
            return rectangleTree.GetItems(playerBigArea);
        }

        private static Entity[] GetSmallList(Entity e)
        {
            Rectangle entitySmallEntity = e.BoundingBox;
            entitySmallEntity.Inflate(50, 50);

            return rectangleTree.GetItems(entitySmallEntity);
        }

        private static Entity[,] GetCloseStatics(Entity e)
        {
            int x = (int)(e.X / Runtime.TileSize);
            int y = (int)(e.Y / Runtime.TileSize);

            return matrix.GetRegion(new Rectangle(x-1, y-1, 3, 3));
        }
    }
}
