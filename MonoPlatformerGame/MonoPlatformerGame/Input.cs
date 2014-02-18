using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }

    public class Input
    {
        private static KeyboardState mNewKey;
        private static KeyboardState mOldKey;

        private static MouseState mOldMouse;
        private static MouseState mNewMouse;

        private static PlayerIndex mPlayerIndex;

        private static GamePadState mNewButton;
        private static GamePadState mOldButton;

		private static Dictionary<string, Keys> mKeybinds;

        public static void Init()
        {
            mNewKey = Keyboard.GetState();
            mOldKey = Keyboard.GetState();

            mOldMouse = Mouse.GetState();
            mNewMouse = Mouse.GetState();

            mPlayerIndex = PlayerIndex.One; //One for now

            mNewButton = GamePad.GetState(mPlayerIndex);
            mOldButton = GamePad.GetState(mPlayerIndex);

			mKeybinds = new Dictionary<string, Keys>();
        }

        public static void Update()
        {
            mOldKey = mNewKey;
            mOldButton = mNewButton;
            mOldMouse = mNewMouse;

            mNewKey = Keyboard.GetState();
            mNewButton = GamePad.GetState(mPlayerIndex);
            mNewMouse = Mouse.GetState();

        }

		public static void RegisterKeybind(string name, Keys key)
		{
			mKeybinds.Add(name, key);
		}

        public static bool IsKeyDown(Keys key)
        {
            return mNewKey.IsKeyDown(key);
        }

		public static bool IsKeyDown(string boundKeyName)
		{
			Keys key;

			mKeybinds.TryGetValue(boundKeyName, out key);

			//if(key != null)
				return IsKeyDown(key);

			return false;
		}

        public static bool IsKeyPressed(Keys key)
        {
            return mNewKey.IsKeyDown(key) && mOldKey.IsKeyUp(key);
        }

        public static bool IsButtonDown(Buttons button)
        {
            return mNewButton.IsButtonDown(button);
        }

        public static bool IsButtonPressed(Buttons button)
        {
            return mNewButton.IsButtonDown(button) && mOldButton.IsButtonUp(button);
        }

        public static bool IsMouseDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return mNewMouse.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return mNewMouse.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return mNewMouse.RightButton == ButtonState.Pressed;
            }
            return false;
        }

        public static bool IsMousePressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return mNewMouse.LeftButton == ButtonState.Pressed && mOldMouse.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return mNewMouse.MiddleButton == ButtonState.Pressed && mOldMouse.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return mNewMouse.RightButton == ButtonState.Pressed && mOldMouse.RightButton == ButtonState.Released;
            }
            return false;
        }

        public static int MouseX()
        {
            return mNewMouse.X;
        }

        public static int MouseY()
        {
            return mNewMouse.Y;
        }

    }
}