using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Interface.ObServers
{
    public interface IChatObserver : IGrainObserver
    {
        void ReceiveMessage(string message);
    }
}
