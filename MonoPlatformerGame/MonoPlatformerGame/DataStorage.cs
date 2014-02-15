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


    }

    class DataStorage
    {
        private static PlayerConfig mPlayerConfig;

        public static PlayerConfig GetLocalPlayerConfig()
        {
            if (mPlayerConfig == null)
                LoadPlayerConfig();

            return mPlayerConfig;
        }
        private static void LoadPlayerConfig()
        {
            PlayerConfig playerConfig = new PlayerConfig();

            try
            {
                string path = Directory.GetCurrentDirectory() + "/PlayerInfo";

                Stream stream = File.OpenRead(path);
                XDocument xml = XDocument.Load(stream);

                XElement xPlayer = xml.Document.Element(XName.Get("Player"));
                XElement xPlayerInfo = xPlayer.Element(XName.Get("PlayerInfo"));
                XElement xServerInfo = xPlayer.Element(XName.Get("ServerInfo"));

                string userName = xPlayerInfo.Element(XName.Get("UserName")).Value;

                if(userName == "Null")
                    userName = System.Environment.MachineName;

                string serverIP = xServerInfo.Element(XName.Get("ServerIP")).Value;
                int serverPort = int.Parse(xServerInfo.Element(XName.Get("ServerPort")).Value);

                playerConfig.UserName = userName;
                playerConfig.ServerIP = serverIP;
                playerConfig.ServerPort = serverPort;
            }
            catch (Exception)
            {
                playerConfig.UserName = System.Environment.MachineName;
                playerConfig.ServerPort = 2300;
                playerConfig.ServerIP = "127.0.0.1";
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
