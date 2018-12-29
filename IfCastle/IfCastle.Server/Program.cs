using IfCastle.Grain;
using IfCastle.Interface;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IfCastle.Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            RunMainAsync().Wait();
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();

                Console.WriteLine("Press Q to terminate...");
                string inputstr = null;
                while (inputstr != "Q")
                {
                    inputstr = Console.ReadLine();
                }
                await host.StopAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;//error result
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var configuration = new ConfigurationBuilder()
                   .AddJsonFile("cluster.json")
                   .Build();
            var config = configuration.Get<GameClusterOptions>();

            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                //.UseAdoNetClustering(options =>
                //{
                //    options.Invariant = config.Clustering.Invariant;
                //    options.ConnectionString = config.Clustering.ConnectionString;
                //})
                .AddMemoryGrainStorageAsDefault()
                //.AddAdoNetGrainStorage("Default", options =>
                //{
                //    options.Invariant = config.Storage.Invariant;
                //    options.ConnectionString = config.Storage.ConnectionString;
                //    options.UseJsonFormat = true;
                //})
               .Configure<ClusterOptions>(options =>
               {
                   options.ClusterId = config.ClusterId;
                   options.ServiceId = config.ServiceId;
               })
               .Configure<EndpointOptions>(options =>
               {
                   //options.GatewayPort = config.Endpoint.GatewayPort;
                   //options.SiloPort = config.Endpoint.SiloPort;
                   options.AdvertisedIPAddress = IPAddress.Loopback;
               })
               //.Configure<EndpointOptions>(options =>
               //{
               //    options.GatewayPort = config.Endpoint.GatewayPort;
               //    options.SiloPort = config.Endpoint.SiloPort;
               //    options.AdvertisedIPAddress = IPAddress.Parse(config.IP);
               //})
               .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences())
               .ConfigureLogging(logging => logging.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true }))
               .AddMemoryGrainStorage("PubSubStore")
               .AddSimpleMessageStreamProvider(Constants.GameRoomStreamProvider);

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
