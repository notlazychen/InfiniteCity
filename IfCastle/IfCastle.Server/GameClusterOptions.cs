using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Server
{
    public class GameClusterOptions
    {
        public string Name { get; set; }
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
        public AdoNetOptions Clustering { get; set; }
        public AdoNetOptions Storage { get; set; }

        public EndpointOptions Endpoint { get; set; }
    }

    public class AdoNetOptions
    {
        public string Invariant { get; set; }
        public string ConnectionString { get; set; }
    }
}
