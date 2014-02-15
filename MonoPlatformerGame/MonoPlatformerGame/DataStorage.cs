using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MonoPlatformerGame
{
    class PlayerInfo
    {
        public string UserName { get; set; }

        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
    }
    class DataStorage
    {
        private static PlayerInfo mPlayerInfo;

        public static PlayerInfo GetLocalPlayerInfo()
        {
            if (mPlayerInfo == null)
                LoadPlayerInfo();

            return mPlayerInfo;
        }
        private static void LoadPlayerInfo()
        {
            PlayerInfo playerInfo = new PlayerInfo();

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

                playerInfo.UserName = userName;
                playerInfo.ServerIP = serverIP;
                playerInfo.ServerPort = serverPort;
            }
            catch (Exception)
            {
                playerInfo.UserName = System.Environment.MachineName;
                playerInfo.ServerPort = 2300;
                playerInfo.ServerIP = "127.0.0.1";
                SavePlayerData(playerInfo);
            }
            mPlayerInfo = playerInfo;
        }

        public static void SaveCurrentPlayerData()
        {
            SavePlayerData(mPlayerInfo);
        }

        private static void SavePlayerData(PlayerInfo playerInfo)
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
