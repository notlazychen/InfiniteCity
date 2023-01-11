using Microsoft.Extensions.Configuration;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IfCastle.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using(IClient client = new Client4Net())
                {
                    client.ConnectAsync("ws://127.0.0.1:8088").Wait();
                    //client.ConnectAsync("ws://101.132.118.172:8088").Wait();

                    Console.WriteLine("连接成功, 请输入命令回车结束, 请选择:");
                    Console.WriteLine("--------");
                    Console.WriteLine($"1 创建房间");
                    Console.WriteLine($"2 加入房间");
                    Console.WriteLine("--------");
                    string c = Console.ReadLine();
                    if(c == "1")
                    {
                        client.SendAsync("create").Wait();
                    }
                    else
                    {
                        Console.WriteLine("请输入房间号");
                        c = Console.ReadLine();
                        client.SendAsync($"enter {c}").Wait();
                    }
                    Console.Clear();
                    Console.WriteLine("--------");
                    Console.WriteLine($"↑");
                    Console.WriteLine($"↑");
                    Console.WriteLine($"↓");
                    Console.WriteLine($"←");
                    Console.WriteLine($"→");
                    Console.WriteLine("按Q结束");
                    Console.WriteLine("--------");
                    while (true)
                    {
                        var x = Console.ReadKey();
                        if (x.Key == ConsoleKey.Q)
                        {
                            break;
                        }
                        switch (x.Key)
                        {
                            case ConsoleKey.UpArrow:
                                client.SendAsync("↑").Wait();
                                break;
                            case ConsoleKey.DownArrow:
                                client.SendAsync("↓").Wait();
                                break;
                            case ConsoleKey.LeftArrow:
                                client.SendAsync("←").Wait();
                                break;
                            case ConsoleKey.RightArrow:
                                client.SendAsync("→").Wait();
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
