﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleChatServer.Servers;

namespace SimpleChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            
            Console.ReadKey();
        }
    }
}
