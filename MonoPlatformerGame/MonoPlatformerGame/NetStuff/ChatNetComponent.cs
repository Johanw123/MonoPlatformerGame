using System;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace MonoPlatformerGame
{
	public class ChatNetComponent : NetComponent
	{
		public bool ChatMode{ get; set; }
		private string chatString = ":";
		private KeyboardState keyOld;
		private KeyboardState keyNew;
        private List<string> chatMessages = new List<string>();
        private List<KeyValuePair<string, float>> chatMessagesFade = new List<KeyValuePair<string, float>>();
		private bool IsAdmin = false;
        
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

            AddChatMessage(playerName, message);
		}

        private void AddChatMessage(string playerName, string message)
        {
            chatMessagesFade.Add(new KeyValuePair<string, float>(playerName + " : " + message, 1.0f));
            chatMessages.Add(playerName + " : " + message);
            JapeLog.WriteLine(playerName + " : " + message);
        }
		private void AddCommandChatMessage(string message)
		{
			chatMessagesFade.Add(new KeyValuePair<string, float>("Command" + " : " + message, 1.0f));
			chatMessages.Add("Command" + " : " + message);
			JapeLog.WriteLine("Command" + " : " + message);
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
					if(chatMessage.Length > 0)
					{
						if(chatMessage.Substring(0, 1) == ".")
						{
							//commands
							if(chatMessage.Length > 2)
							{
								switch(chatMessage.Substring(1).ToUpper())
								{
									case "POKEMON":
										IsAdmin = true;
										AddCommandChatMessage("Admin mode activated");
										return;
									case "START":
										if(IsAdmin)
										{
											AddCommandChatMessage("Starting Game");
											NetManager.SendMessageParams(NetDeliveryMethod.ReliableOrdered,
											                            (int)DataType.StartGame
											);
										}
										else
											AddCommandChatMessage("You are not Admin...");
									
										break;

								}
							}


						}
						NetManager.SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableOrdered,
						                                                      (int)DataType.ChatMessage,
						                                                      DataStorage.GetLocalPlayerConfig().UserName,
						                                                      chatMessage
						);

						AddChatMessage(DataStorage.GetLocalPlayerConfig().UserName, chatMessage);
						//NetOutgoingMessage oMsg = NetManager.CreateMessage();
						//oMsg.Write((int)DataType.ChatMessage);
						//oMsg.Write(chatMessage);
						//NetManager.SendMessage(NetDeliveryMethod.ReliableOrdered, oMsg);
					}
					else
					{
						keyboardManager.Text = ": ";
						keyTest.output = ": ";
					}
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

                int j = 1;
                for (int i = chatMessages.Count - 1; i >= 0; --i)
                {
					spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatMessages[i], new Vector2(Runtime.ScreenWidth/2 - 200, (Runtime.ScreenHeight-50) - j * 30) + new Vector2(1.5f,1.5f), Color.Black);
                    spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatMessages[i], new Vector2(Runtime.ScreenWidth/2 - 200, (Runtime.ScreenHeight-50) - j * 30), Color.DodgerBlue);
                    ++j;
                }
                //for (int i = 0; i < chatMessages.Count; ++i)
                //{
                //    spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatMessages[i], new Vector2(500, 500 - i * 30), Color.White);
                //}

				if(chatString != null)
				{
					spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatString, new Vector2(Runtime.ScreenWidth / 2 - 200, (Runtime.ScreenHeight - 50)) + new Vector2(1.5f,1.5f), Color.Black);
					spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatString, new Vector2(Runtime.ScreenWidth / 2 - 200, (Runtime.ScreenHeight - 50)), Color.DodgerBlue);
				}
				spriteBatch.End();
			}
            else
            {
                int j = 1;
                for (int i = chatMessagesFade.Count - 1; i >= 0; --i)
                {
                    spriteBatch.Begin();
					spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatMessagesFade[i].Key, new Vector2(Runtime.ScreenWidth / 2 - 200, (Runtime.ScreenHeight - 50) - j * 30) + new Vector2(1.5f,1.5f), Color.Black * chatMessagesFade[i].Value);
					spriteBatch.DrawString(ResourceManager.GetFont("Verdana"), chatMessagesFade[i].Key, new Vector2(Runtime.ScreenWidth / 2 - 200, (Runtime.ScreenHeight - 50) - j * 30), Color.DodgerBlue * chatMessagesFade[i].Value);
                    ++j;
                    spriteBatch.End();
                }
                if (chatMessages.Count > 20)
                {
                    chatMessages.RemoveAt(0);
                }
            }

            for (int i = 0; i < chatMessagesFade.Count; i++)
            {
                chatMessagesFade[i] = new KeyValuePair<string, float>(chatMessagesFade[i].Key, chatMessagesFade[i].Value - 0.005f);
                if (chatMessagesFade[i].Value <= 0)
                {
                    chatMessagesFade.Remove(chatMessagesFade[i]);
                }
            }
            
		}

	}
}