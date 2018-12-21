using IfCastle.Interfaces.ObServers;
using Orleans;
using System;
using System.Threading.Tasks;

namespace IfCastle.Interfaces
{
    public interface IChat: IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
        Task Subscribe(IChatObserver chatClient);
    }
}
