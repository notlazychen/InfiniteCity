using IfCastle.Interface.ObServers;
using Orleans;
using System;
using System.Threading.Tasks;

namespace IfCastle.Interface
{
    public interface IChat: IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
        Task Subscribe(IChatObserver chatClient);
    }
}
