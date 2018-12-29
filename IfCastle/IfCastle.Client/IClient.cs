using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Client
{
    public interface IClient :IDisposable
    {
        Task ConnectAsync(string uri);

        Task SendAsync(string msg);

        int NetDelay { get; }
    }
}
