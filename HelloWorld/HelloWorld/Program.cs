using Hello.Grain;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using Orleans;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace HelloWorld
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json")
                     .Build();
                var config = configuration.GetSection("SiloHost").Get<SiloHostConfig>();
                var host = await StartSilo(config);
                Console.WriteLine("Press Q and Enter to terminate...");

                while (true)
                {
                    string q = Console.ReadLine();
                    if(string.Compare(q, "q", true) == 0)
                    {
                        break;
                    }
                }
                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo(SiloHostConfig config)
        {
            var builder = new SiloHostBuilder()
                 .UseAdoNetClustering(op =>
                 {
                     op.ConnectionString = config.ClusteringDbConnectionString;
                     op.Invariant = config.ClusteringDbInvariant;
                 })                 
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = config.ClusterId;
                    options.ServiceId = config.ServiceId;
                })
                .Configure<EndpointOptions>(options =>
                {
                    options.AdvertisedIPAddress = IPAddress.Parse(config.AdvertisedIPAddress);
                    options.SiloPort = config.SiloPort;
                    options.GatewayPort = config.GatewayPort;
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddDebug());

            if (config.Dashboard != null && config.Dashboard.UseDashboard)
            {
                builder = builder.UseDashboard(options =>
                  {
                      options.Username = config.Dashboard.Username;
                      options.Password = config.Dashboard.Password;
                      options.Host = config.Dashboard.Host;
                      options.Port = config.Dashboard.Port;
                      options.HostSelf = config.Dashboard.HostSelf;
                      options.CounterUpdateIntervalMs = config.Dashboard.CounterUpdateIntervalMs;
                  });
            }
            
            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
