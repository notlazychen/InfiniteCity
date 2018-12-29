using Fleck;
using IfCastle.Interface;
using IfCastle.Interface.Model;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace IfCastle.Gateway
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var server = new GatewayServer("ws://0.0.0.0:8088"))
                {
                    server.Start();
                    Console.WriteLine("服务器启动成功，输入q退出");
                    while (true)
                    {
                        var x = Console.ReadLine();
                        if (x == "q")
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

    }
}
