using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Gateway
{
    public class ServiceOptions
    {
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
        public string Invariant { get; set; }
        public string ConnectionString { get; set; }
    }
}
