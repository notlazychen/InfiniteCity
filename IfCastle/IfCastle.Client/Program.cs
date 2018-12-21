using IfCastle.Interfaces;
using Microsoft.Extensions.Configuration;
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
                    var friend = client.GetGrain<IChat>(0);
                    string inputstr = null;
                    while (inputstr != "Q")
                    {
                        inputstr = Console.ReadLine();
                        var response = friend.SayHello(inputstr).Result;
                        Console.WriteLine("\n\n{0}\n\n", response);
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
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = ops.Invariant;
                    options.ConnectionString = ops.ConnectionString;
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = ops.ClusterId;
                    options.ServiceId = ops.ServiceId;
                })
                .ConfigureLogging(logging => logging.AddNLog())
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
