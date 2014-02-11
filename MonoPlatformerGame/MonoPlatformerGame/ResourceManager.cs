using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonoPlatformerGame
{
    class ResourceManager
    {
        static Dictionary<string, Texture2D> mTextures;
        static Dictionary<string, SpriteFont> mFonts;

        public static Texture2D GetTexture(string name)
        {
            Texture2D texture;

            mTextures.TryGetValue(name, out texture);

            return texture;

        }
        public static SpriteFont GetFont(string name)
        {
            SpriteFont font;

            mFonts.TryGetValue(name, out font);

            return font;
        }

        public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            mTextures = new Dictionary<string, Texture2D>();
            mFonts = new Dictionary<string, SpriteFont>();


            //LoadFolder("", content);
            LoadFolder("Sprites", content);
            LoadFontsFolder("Fonts", content);
            LoadSpecialTextures(graphicsDevice);
        }

        private static void LoadSpecialTextures(GraphicsDevice graphicsDevice)
        {
            Texture2D tex = new Texture2D(graphicsDevice, 2, 2);
            tex.SetData(new Color[] { Color.White, Color.White, Color.White, Color.White });
            mTextures.Add("pixel", tex);
        }

        private static void LoadFontsFolder(string folder, ContentManager content)
        {
            string[] files = Directory.GetFiles("Content/" + folder);

            foreach (string file in files)
            {
				string fileName;

                if (file.Contains('\\'))
                    fileName = file.Substring(file.LastIndexOf('\\') + 1);
                else
                    fileName = file.Substring(file.LastIndexOf('/') + 1);

                fileName = fileName.Remove(fileName.LastIndexOf('.'));
                mFonts.Add(fileName, content.Load<SpriteFont>(folder + '/' + fileName));
            }
        }

        private static void LoadFolder(string folder, ContentManager content)
        {
            if (!Directory.Exists("Content/" + folder))
                return;

            string[] files = Directory.GetFiles("Content/" + folder);

            foreach (string file in files)
            {
				string fileName;

                if(file.Contains('\\'))
                    fileName = file.Substring(file.LastIndexOf('\\') + 1);
                else
                    fileName = file.Substring(file.LastIndexOf('/') + 1);

                fileName = fileName.Remove(fileName.LastIndexOf('.'));
                mTextures.Add(fileName, content.Load<Texture2D>(folder + '/' + fileName));
            }
        }


    }
}
