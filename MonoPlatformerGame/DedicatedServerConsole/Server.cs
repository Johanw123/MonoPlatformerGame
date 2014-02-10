﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MonoPlatformerGame;
using System.Threading;

namespace DedicatedServerConsole
{
    class Server
    {
        bool run;
        DedicatedServerNetComponent dedicatedServerNetComponent;
        Thread commandsThread;
        Log log;

        public Server()
        {
            run = true;
            commandsThread = new Thread(ListenForCommands);
            commandsThread.Start();

            log = new JapeLog();
            Log.Init(log);

            NetManager.Init(true);
            NetManager.AddComponent(dedicatedServerNetComponent = new DedicatedServerNetComponent());
            NetManager.IsDedicatedHost = true;

        }
        private void ListenForCommands()
        {
            while (run)
            {
                string input = Console.ReadLine();

				if (input == null)
					continue;

                string[] seperatedInput = input.Split(' ');
                string command = "";
                List<string> commandArgs = new List<string>();

                for (int i = 0; i < seperatedInput.Length; i++)
                {
                    if (i == 0)
                        command = seperatedInput[i];
                    else
                        commandArgs.Add(seperatedInput[i]);
                }

                switch (command.ToUpper())
                {
				case "START":
					StartCommand ();
                    break;
				case "KICK":
					KickCommand (commandArgs);
					break;
				case "BAN":
					BanCommand (commandArgs);
					break;
				case "CONNECTEDPLAYERS":
				case "LISTPLAYERS":
				case "PLAYERS":
				case "CLIENTS":
				case "LISTCLIENTS":
				case "CONNECTEDCLIENTS":
					ListClientsCommand (commandArgs);
					break;
				case "/T":
				case "/W":
				case "CHAT":
				case "SPEAK":
				case "TELL":
				case "SAY":
					SayCommand (commandArgs);
					break;
				case "CHANGEMAP":
				case "MAP":
				case "CHANGELEVEL":
				case "LEVEL":
					ChangeLevelCommand (commandArgs);
                    break;
                }
            }
        }

		private void StartCommand()
		{
			NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
			                             (int)DataType.StartGame
			                             );
		}
		private void ChangeLevelCommand(List<string> commandArgs)
		{
			if (commandArgs.Count > 0)
			{
				string levelName = commandArgs[0] + ".tmx";

				NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
				                             (int)DataType.ChangeLevel,
				                             levelName
				                             );
			}
		}

		private void KickCommand(List<string> commandArgs)
		{
			if (commandArgs.Count > 0)
			{
				string playerNameOrId = commandArgs [0];

				int value;
				if (int.TryParse(playerNameOrId, out value))
				{
					// Parse successful, value can be id.
					//TODO check if id is valid (connected etc).
					NetManager.KickPlayer (value);
				}
				else
				{
					NetManager.KickPlayer(playerNameOrId);
				}
			}
		}

		private void BanCommand(List<string> commandArgs)
		{
			if (commandArgs.Count > 0)
			{
				string playerName = commandArgs [0];




			}
		}

		private void SayCommand(List<string> commandArgs)
		{
			if (commandArgs.Count > 0)
			{
				string message = commandArgs[0];

				NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
			                             		(int)DataType.ChatMessage,
	                             				message
				                             	);
			}
		}

		private void ListClientsCommand(List<string> commandArgs)
		{
			foreach (var client in NetManager.connectedClients)
			{
				string name = client.Value.Name;
				string id = client.Value.UID.ToString();

				Console.WriteLine ("{0} : {1}", name, id);
			}
		}

        public void Run()
        {
            while (run)
            {
                NetManager.Listen();
                dedicatedServerNetComponent.Update();
            }
        }
    }
}
