using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MonoPlatformerGame
{
    class PlayerConfig
    {
        public string UserName { get; set; }

        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
    }

    class ServerConfig
    {
        public string ServerName { get; set; }
        public string GameMode { get; set; }
        public int HostingPort { get; set; }
		public List<string> LevelRotation { get; set; }



    }

    class DataStorage
    {
        private static PlayerConfig mPlayerConfig;
		private static ServerConfig mServerConfig;

        public static PlayerConfig GetLocalPlayerConfig()
        {
            if (mPlayerConfig == null)
                LoadPlayerConfig();

            return mPlayerConfig;
        }

		public static ServerConfig GetLocalServerConfig()
		{
			if(mServerConfig == null)
				LoadServerConfig();

			return mServerConfig;
		}

		private static void LoadServerConfig()
		{
			ServerConfig serverConfig = new ServerConfig();

			try
			{
				string path = Directory.GetCurrentDirectory() + "/ServerConfig";

				Stream stream = File.OpenRead(path);
				XDocument xml = XDocument.Load(stream);

				XElement xServer= xml.Document.Element(XName.Get("Server"));
				XElement xServerConfig = xServer.Element(XName.Get("ServerConfig"));
				XElement xLevelRotation = xServer.Element(XName.Get("LevelRotation"));

				string serverName = xServerConfig.Element(XName.Get("ServerName")).Value;
				string gameMode = xServerConfig.Element(XName.Get("GameMode")).Value; 

				if(serverName == "Null")
					serverName = System.Environment.MachineName + "'s Server";  

				//string serverIP = xServerInfo.Element(XName.Get("ServerIP")).Value;
				int hostingPort = int.Parse(xServerConfig.Element(XName.Get("HostingPort")).Value);

				serverConfig.ServerName = serverName;
				serverConfig.HostingPort = hostingPort;
				serverConfig.GameMode = "";



			}
			catch(Exception)
			{
				serverConfig.ServerName = System.Environment.MachineName + "'s Server";
				serverConfig.HostingPort = 2300;
				serverConfig.GameMode = "Race";
                serverConfig.LevelRotation = new List<string>
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
				//SavePlayerData(playerConfig);
			}
			mServerConfig = serverConfig;
		}

        private static void LoadPlayerConfig()
        {
            PlayerConfig playerConfig = new PlayerConfig();

            try
            {
                string path = Directory.GetCurrentDirectory() + "/PlayerConfig";

                Stream stream = File.OpenRead(path);
                XDocument xml = XDocument.Load(stream);

                XElement xPlayer = xml.Document.Element(XName.Get("Player"));
                XElement xPlayerConfig = xPlayer.Element(XName.Get("PlayerConfig"));
                XElement xServerConfig = xPlayer.Element(XName.Get("ServerConfig"));

                string userName = xPlayerConfig.Element(XName.Get("UserName")).Value;

                if(userName == "Null")
                    userName = System.Environment.MachineName;

                string serverIP = xServerConfig.Element(XName.Get("ServerIP")).Value;
                int serverPort = int.Parse(xServerConfig.Element(XName.Get("ServerPort")).Value);

                playerConfig.UserName = userName;
                playerConfig.ServerIP = serverIP;
                playerConfig.ServerPort = serverPort;
            }
            catch (Exception)
            {
                playerConfig.UserName = System.Environment.MachineName;
                playerConfig.ServerPort = 2300;
                playerConfig.ServerIP = "90.224.75.79";
                SavePlayerData(playerConfig);
            }
            mPlayerConfig = playerConfig;
        }

        public static void SaveCurrentPlayerData()
        {
            SavePlayerData(mPlayerConfig);
        }

        private static void SavePlayerData(PlayerConfig playerInfo)
        {
            XElement xmlTree = new XElement("Player");

            xmlTree.Add(new XElement("PlayerInfo",
                           new XElement("UserName", playerInfo.UserName)
                           ));

            xmlTree.Add(new XElement("ServerInfo",
                           new XElement("ServerIP", playerInfo.ServerIP),
                           new XElement("ServerPort", playerInfo.ServerPort)
                           ));


            XDocument document = new XDocument(xmlTree);
            document.Declaration = new XDeclaration("1.0", "utf-8", "true");
            document.Save("PlayerInfo");

        }

    }
}
