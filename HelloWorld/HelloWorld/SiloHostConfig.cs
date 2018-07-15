using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
    class SiloHostConfig
    {
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
        public string ClusteringDbConnectionString { get; set; }
        public string ClusteringDbInvariant { get; set; }
        public string AdvertisedIPAddress { get; set; }
        public int SiloPort { get; set; }
        public int GatewayPort { get; set; }
        public DashboardConfig Dashboard { get; set; }
    }

    class DashboardConfig
    {
        public bool UseDashboard { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool HostSelf { get; set; }
        public int CounterUpdateIntervalMs { get; set; }
    }
}
