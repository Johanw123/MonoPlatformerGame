using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MonoPlatformerGame;
using System.Threading;
using System.IO;
using Lidgren.Network;

namespace DedicatedServerConsole
{
	public enum GameMode
	{
		//You have a cerain time to finish the course and the best time wins (track-mania style).
		//several tries, dying and reaching goal will put player back at start to try again
		TimeTrial,

		//First to the goal is the winner. Also if all dies, next map will be loaded
		//A fast pased game with high tempo
		Race,

		//Screen scrolls the same for all players. One life only, survivers that reach the goal are rewarded with a point.
		//Most point at end of certain ammount of levels will win.
		//(Coop/Versus?)
		Survival
	}

    class Server
    {
		public static GameMode CurrentGameMode { get; set; }
        bool run;
		int curr = 0;
        DedicatedServerNetComponent dedicatedServerNetComponent;
        Thread commandsThread;
        Log log;
        public string CurrentLevelName { get; set; }
		List<string> mapRotation = new List<string>{
			"Race1.tmx",
			"Race2.tmx",
			"Race3.tmx",
			"Race4.tmx",
			"Race5.tmx"
		};

        public Server()
        {
            run = true;
            //CurrentLevelName = "Level.tmx";
			CurrentLevelName = mapRotation[0];
            NetManager.CurrentLevelName = CurrentLevelName;
			CurrentGameMode = GameMode.Race;
            commandsThread = new Thread(ListenForCommands);
            commandsThread.Start();

            log = new JapeLog();
            Log.Init(log);

			dedicatedServerNetComponent = new DedicatedServerNetComponent();
			dedicatedServerNetComponent.NextLevelEvent += NextLevel;

            NetManager.Init(true);
			NetManager.AddComponent(dedicatedServerNetComponent);
            NetManager.IsDedicatedHost = true;    
        }


		private void NextLevel()
		{
			++curr;
			if(curr >= mapRotation.Count)
				curr = 0;

			string levelName = mapRotation[curr];

			ChangeLevel(levelName);
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
                        StartCommand();
                        break;
                    case "KICK":
                        KickCommand(commandArgs);
                        break;
                    case "BAN":
                        BanCommand(commandArgs);
					    break;
                    case "LISTLEVELS":
                    case "LEVELS":
                    case "LISTMAPS":
                    case "MAPS":
                        ListLevelsCommand(commandArgs);
                        break;
                    case "CONNECTEDPLAYERS":
                    case "LISTPLAYERS":
                    case "PLAYERS":
                    case "CLIENTS":
                    case "LISTCLIENTS":
                    case "CONNECTEDCLIENTS":
                        ListClientsCommand(commandArgs);
                        break;
                    case "/T":
                    case "/W":
                    case "CHAT":
                    case "SPEAK":
                    case "TELL":
                    case "SAY":
                        SayCommand(commandArgs);
                        break;
                    case "CHANGEMAP":
                    case "MAP":
                    case "CHANGELEVEL":
                    case "LEVEL":
                        ChangeLevelCommand(commandArgs);
                        break;
                }
            }
        }

		private void StartCommand()
		{
			ChangeLevel(CurrentLevelName);

            NetManager.StartGame();
		}

		private void ChangeLevelCommand(List<string> commandArgs)
		{
			if (commandArgs.Count > 0)
			{
                string levelName = commandArgs[0] + ".tmx";

				ChangeLevel(levelName);
			}
		}

		private void ChangeLevel(string levelName)
		{
			string path = "Maps/" + levelName;

			if (File.Exists(path))
			{
				//TODO
				//Läsa in och kolla game-mode...
				string levelData = File.ReadAllText(path);
				NetManager.SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableOrdered,
				                                        (int)DataType.ChangeLevel,
				                                        levelName,
				                                        levelData
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
					NetManager.KickPlayer(value);
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
				string playerName = commandArgs[0];




			}
		}

		private void SayCommand(List<string> commandArgs)
		{
			if (commandArgs.Count > 0)
			{
				string message = commandArgs[0];

				/*
				  NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
			                             		(int)DataType.ChatMessage,
				                             	"Host",
	                             				message
				                             	);



				Lidgren.Network.NetOutgoingMessage msg = NetManager.CreateMessage();
				msg.Write((int)DataType.ChatMessage);
				msg.Write("Host");
				msg.Write(message);
				NetManager.SendBroadcast(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
*/
				NetManager.SendMessageParamsStringsOnly(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
				                                        (int)DataType.ChatMessage,
				                                        "Host",
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

        private void ListLevelsCommand(List<string> commandArgs)
        {
            string[] files = Directory.GetFiles("Maps/");
            string fileName;
            int id = -1;

            foreach (string file in files)
            {
                if (file.Contains("\\"))
                    fileName = file.Substring(file.LastIndexOf('\\') + 1);
                else
                    fileName = file.Substring(file.LastIndexOf('/') + 1);

                fileName = fileName.Remove(fileName.LastIndexOf('.'));

                Console.WriteLine(++id + " - " + "'" + fileName + "'");
            }
        }

        public void Run()
        {
			int framesElapsed = 0;

            while (run)
            {
                NetManager.Listen();
                dedicatedServerNetComponent.Update();
				++framesElapsed;
            }
        }

        

    }
}
