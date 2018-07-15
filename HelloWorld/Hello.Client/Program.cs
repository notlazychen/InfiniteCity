using System;
using System.Threading.Tasks;
using Hello.Interface;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Hello.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            RunAsync().Wait();
        }

        async static Task RunAsync()
        {
            using (var client = await StartClient())
            {
                var hello = client.GetGrain<IHello>(0);

                while (true)
                {
                    string msg = Console.ReadLine();
                    string result = await hello.SayHello(msg);
                    Console.WriteLine(result);
                }
            }
        }

        async static Task<IClusterClient> StartClient()
        {
            var client = new ClientBuilder()
                 .UseAdoNetClustering(op =>
                 {
                     op.ConnectionString = "Server=101.132.118.172;Port=3306;Database=orleans;Uid=root;Pwd=chenrong.123;SslMode=0;";
                     op.Invariant = "MySql.Data.MySqlClient";
                 })
                //.UseConsulClustering(options =>
                //{
                //    options.Address = new Uri("http://localhost:8500");
                //})
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "hello";
                    options.ServiceId = "h1";
                })
                //.UseLocalhostClustering()
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            return client;
        }
    }
}
