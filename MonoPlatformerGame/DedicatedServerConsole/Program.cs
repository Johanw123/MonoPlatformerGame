﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Run();
        }
    }
}
