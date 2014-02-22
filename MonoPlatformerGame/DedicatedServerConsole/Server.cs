using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MonoPlatformerGame;
using System.Threading;
using System.IO;
using Lidgren.Network;
using System.Diagnostics;

namespace DedicatedServerConsole
{
    class Server
    {
        bool run;
		int curr = 0;
        DedicatedServerNetComponent dedicatedServerNetComponent;
        Thread commandsThread;
        Log log;
		List<string> mapRotation = new List<string>
		{
			"Race1.tmx",
			"Race2.tmx",
			"Race3.tmx",
			"Race4.tmx",
			"Race5.tmx",
            "Race6.tmx",
            "Race7.tmx",
            "Race8.tmx",
            "Race9.tmx"
		};

        public Server()
        {
			mapRotation = DataStorage.GetLocalServerConfig().LevelRotation;
            LoadLevel(mapRotation[0]);

            log = new JapeLog();
            Log.Init(log);

			dedicatedServerNetComponent = new DedicatedServerNetComponent();
            dedicatedServerNetComponent.NextLevelEvent += NextLevel;

            NetManager.Init(true);
			NetManager.AddComponent(dedicatedServerNetComponent);
            NetManager.IsHost = true;

			run = true;

            commandsThread = new Thread(ListenForCommands) { IsBackground = true };
			commandsThread.Start();
        }

        private void NextLevel()
        {
            ++curr;
            if (curr >= mapRotation.Count)
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
                    case "SHUTDOWN":
                    case "EXIT":
                        break;

                    case "RESTART":
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
                    case "CRASH":
                    case "SIMULATECRASH":
                        SimulateCrashCommand(commandArgs);
                        break;
                }
            }
        }

		private void StartCommand()
		{
            if (Runtime.CurrentLevel.Loaded)
            {
                string levelName = Runtime.CurrentLevel.Name;
                string levelData = Runtime.CurrentLevel.Data;

                NetManager.SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableOrdered,
                                                               (int)DataType.StartGame,
                                                               Runtime.CurrentLevel.Name,
                                                               levelData
                                                               );
                NetManager.StartGame();
                Console.WriteLine("Game started");
            }
		}

		private void ParseLevelData(string levelData)
		{
			//<property name="DoubleJump" value="0"/>
			string s = getBetween(levelData,"<properties>","</properties>");
			string s2 = getBetween(s, "<property name=\"DoubleJump\" value=\"", "\"/>");

			switch(s2)
			{
				case "Race":
					Runtime.CurrentLevel.GameMode = GameMode.Race;
					break;
				case "TimeTrial":
                    Runtime.CurrentLevel.GameMode = GameMode.TimeTrial;
					break;
				case "Survival":
                    Runtime.CurrentLevel.GameMode = GameMode.Survival;
					break;
				default:
                    Runtime.CurrentLevel.GameMode = GameMode.Race;
					break;
			}

		}

		public static string getBetween(string strSource, string strStart, string strEnd)
		{
			int Start, End;
			if (strSource.Contains(strStart) && strSource.Contains(strEnd))
			{
				Start = strSource.IndexOf(strStart, 0) + strStart.Length;
				End = strSource.IndexOf(strEnd, Start);
				return strSource.Substring(Start, End - Start);
			}
			else
			{
				return "";
			}
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
            if (LoadLevel(levelName))
            {
                NetManager.SendMessageParamsStringsOnly(NetDeliveryMethod.ReliableOrdered,
                                                            (int)DataType.ChangeLevel,
                                                            Runtime.CurrentLevel.Name,
                                                            Runtime.CurrentLevel.Data,
                                                            "3.0" //Timed delay to start next level
                                                            );

                Console.WriteLine("Level Changed to: " + Runtime.CurrentLevel.Name);
            }
		}

        private bool LoadLevel(string levelName)
        {
            string path = "Maps/" + levelName;
            string levelData = GetLevelData(path);

            if (!string.IsNullOrWhiteSpace(levelData) && levelData != "Null")
            {
                ParseLevelData(levelData);
                Runtime.CurrentLevel.Data = levelData;
                Runtime.CurrentLevel.Name = levelName;
                Runtime.CurrentLevel.Loaded = true;
                return true;
            }
            return false;
        }

        private string GetLevelData(string fullPathOrFileName)
        {
            string levelData = "Null";

            if (File.Exists(fullPathOrFileName))
            {
                levelData = File.ReadAllText(fullPathOrFileName);
            }
            else if (File.Exists("Maps/" + fullPathOrFileName))
            {
                levelData = File.ReadAllText("Maps/" + fullPathOrFileName);
            }

            return levelData;
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

        private void SimulateCrashCommand(List<string> commandArgs)
        {
            throw new ExecutionEngineException("This is a simulated crash exception");
        }

    }
}
