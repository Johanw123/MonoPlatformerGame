using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace MonoPlatformerGame
{
    public class ResourceManager
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

        public static void Initialize(ContentManager content)
        {
            mTextures = new Dictionary<string, Texture2D>();
            mFonts = new Dictionary<string, SpriteFont>();


            //LoadFolder("", content);
            LoadFolder("Sprites", content);
            LoadFontsFolder("Fonts", content);
        }

        private static void LoadFontsFolder(string folder, ContentManager content)
        {
            string[] files = Directory.GetFiles("Content/" + folder);

            foreach (string file in files)
            {
                string fileName = file.Substring(file.LastIndexOf('\\') + 1);
                fileName = fileName.Remove(fileName.LastIndexOf('.'));
                mFonts.Add(fileName, content.Load<SpriteFont>(folder + '/' + fileName));
            }
        }

        private static void LoadFolder(string folder, ContentManager content)
        {
            if(!Directory.Exists("Content/" + folder))
                return;

            string[] files = Directory.GetFiles("Content/" + folder);

            foreach (string file in files)
            {
                string fileName = file.Substring(file.LastIndexOf('\\') + 1);
                fileName = fileName.Remove(fileName.LastIndexOf('.'));
                mTextures.Add(fileName, content.Load<Texture2D>(folder + '/' + fileName));
            }
        }

    }
}
