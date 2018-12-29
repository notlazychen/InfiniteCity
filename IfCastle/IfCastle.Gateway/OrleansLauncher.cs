using IfCastle.Interface;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Gateway
{
    public static class OrleansLauncher
    {
        public static async Task<IClusterClient> StartClientWithRetries()
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
            attempt++;
            if (attempt > initializeAttemptsBeforeFailing)
            {
                return false;
            }
            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }
    }
}
