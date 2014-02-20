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


		private KeyboardTest keyTest = new KeyboardTest();
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

		public void Update(GameTime gameTime, KeyboardManager keyboardManager)
		{
			keyOld = keyNew;
			keyNew = Keyboard.GetState();
			keyTest.Update();
            keyboardManager.Update(gameTime);
            chatString = keyboardManager.Text;
			chatString = keyTest.output;
			if(keyNew.IsKeyDown(Keys.Enter) && keyOld.IsKeyUp(Keys.Enter))
			{
				if(ChatMode)
				{
					ChatMode = false;
                    keyboardManager.Text = ": ";
					keyTest.output = ": ";
                    string chatMessage = chatString.Substring(2);

                    NetManager.SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableOrdered,
                                                            (int)DataType.ChatMessage,
                                                            chatMessage
                                                            );

                    //NetOutgoingMessage oMsg = NetManager.CreateMessage();
                    //oMsg.Write((int)DataType.ChatMessage);
                    //oMsg.Write(chatMessage);
                    //NetManager.SendMessage(NetDeliveryMethod.ReliableOrdered, oMsg);
				}
				else
				{
					ChatMode = true;
                    keyboardManager.Text = ": ";
					keyTest.output = ": ";
				}
			}


		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if(ChatMode)
			{
				spriteBatch.Begin();
                if (chatString != null)
				spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatString, new Vector2(100,100), Color.White);
				spriteBatch.End();
			}
		}

	}
}