using IfCastle.Interface;
using IfCastle.Interface.Model;
using IfCastle.Interface.ObServers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace IfCastle.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var client = StartClientWithRetries().Result)
                {
                    Console.WriteLine("连接服务成功");
                    var guid = Guid.NewGuid();
                    var game = client.GetGrain<IBlockGame>(0);
                    //var observer = client.CreateObjectReference<IChatObserver>(new GameObServer()).Result;
                    //game.Subscribe(observer);
                    var streamId = game.Start().Result;

                    var stream = client.GetStreamProvider(Constants.GameRoomStreamProvider).GetStream<GameFrameMsg>(streamId, Constants.GameRoomStreamNameSpace);
                    stream.SubscribeAsync(new GameObServer());

                    while (true)
                    {
                        var x = Console.ReadKey();
                        switch (x.Key)
                        {
                            case ConsoleKey.W:
                                game.Move(Direction.Up).Wait();
                                break;
                            case ConsoleKey.S:
                                game.Move(Direction.Down).Wait();
                                break;
                            case ConsoleKey.A:
                                game.Move(Direction.Left).Wait();
                                break;
                            case ConsoleKey.D:
                                game.Move(Direction.Right).Wait();
                                break;
                        }
                        if(x.Key == ConsoleKey.Q)
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

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            var configuration = new ConfigurationBuilder()
                  .AddJsonFile("cluster.json")
                  .Build();
            var ops = configuration.Get<ServiceOptions>();

            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()                
                //.UseAdoNetClustering(options =>
                //{
                //    options.Invariant = ops.Invariant;
                //    options.ConnectionString = ops.ConnectionString;
                //})
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = ops.ClusterId;
                    options.ServiceId = ops.ServiceId;
                })
                .ConfigureLogging(logging => logging.AddNLog())
                .AddSimpleMessageStreamProvider(Constants.GameRoomStreamProvider)
                .Build();

            await client.Connect(RetryFilter);
            return client;
        }

        private const int initializeAttemptsBeforeFailing = 5;
        private static int attempt = 0;
        private static async Task<bool> RetryFilter(Exception exception)
        {
            //if (exception.GetType() != typeof(SiloUnavailableException))
            //{
            //    //Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
            //    return false;
            //}
            attempt ++;
            //Console.WriteLine($"Cluster client attempt {attempt} of {initializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (attempt > initializeAttemptsBeforeFailing)
            {
                return false;
            }
            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }
    }
}
