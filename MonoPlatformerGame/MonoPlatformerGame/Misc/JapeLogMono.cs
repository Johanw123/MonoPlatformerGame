using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoPlatformerGame
{
    public class JapeLogMono : Log
    {
        static Game game;

        private static List<string> log = new List<string>();

        public JapeLogMono(Game game)
        {
            JapeLogMono.game = game;

            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            font = ResourceManager.GetFont("japelog");
        }

        SpriteFont font;
        SpriteBatch spriteBatch;

        protected override void WriteLineInternal(string line)
        {
            log.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + line);
        }
        //public static void WriteLine(object o)
        //{
        //    log.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + o.ToString());
        //}
        protected override void WriteLineInternal(params object[] a)
        {
            foreach (var o in a)
	        {
                log.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + o.ToString());
	        }
        }

        public override void Draw()
        {
            spriteBatch.Begin();

            int i = log.Count - 10;

            if (i < 0)
                i = 0;

            int row = 0;

            for (; i < log.Count; i++)
            {
                float height = font.MeasureString(log[i]).Y;

                spriteBatch.DrawString(font, log[i], new Vector2(11, 11 + height * row), Color.Black * 0.5f);
                spriteBatch.DrawString(font, log[i], new Vector2(10, 10 + height * row), Color.White * 0.5f);

                row++;
            }

            spriteBatch.End();
        }
    }
}
