using System;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace MonoPlatformerGame
{
	public class ChatNetComponent : NetComponent
	{
		public bool ChatMode{ get; set; }
		private string chatString = "|";
		private KeyboardState keyOld;
		private KeyboardState keyNew;
		private Stopwatch inputTimer = new Stopwatch();

		public ChatNetComponent()
		{
			inputTimer.Start();
		}

		public override bool IncomingData(DataType type, NetIncomingMessage msg)
		{
			switch (type)
			{
				case DataType.ChatMessage:
					IncomingChatMessage(msg);
				return true;
			}
			return false;
		}

		void IncomingChatMessage(NetIncomingMessage msg)
		{
			string playerName = msg.ReadString();
			string message = msg.ReadString();

			JapeLog.WriteLine(playerName + " : " + message);
		}

		public void Update()
		{
			keyOld = keyNew;
			keyNew = Keyboard.GetState();

			var a = keyNew.GetPressedKeys();
			if(a.Length > 0 && inputTimer.ElapsedMilliseconds > 150)
			{
				chatString += a[0].ToString();
				inputTimer.Restart();
			}

			if(keyNew.IsKeyDown(Keys.Enter) && keyOld.IsKeyUp(Keys.Enter))
			{
				if(ChatMode)
				{
					ChatMode = false;
					chatString = "|";
				}
				else
				{
					chatString = "|";
					ChatMode = true;
				}
			}


		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if(ChatMode)
			{
				spriteBatch.Begin();
				spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatString, new Vector2(100,100), Color.White);
				spriteBatch.End();
			}
		}

	}
}