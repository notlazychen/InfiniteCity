using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Interfaces.ObServers
{
    public interface IChatObserver : IGrainObserver
    {
        void ReceiveMessage(string message);
    }
}
