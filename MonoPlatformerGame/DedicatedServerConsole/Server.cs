using System;
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

                switch (command)
                {
                    case "Start":
                        NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                            (int)DataType.StartGame
                            );
                        break;
                    case "Level":
                        if (commandArgs.Count > 0)
                        {
                            string levelName = commandArgs[0] + ".tmx";

                            NetManager.SendMessageParams(Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                                (int)DataType.ChangeLevel,
                                levelName
                                );
                        }
                        break;
                }
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
