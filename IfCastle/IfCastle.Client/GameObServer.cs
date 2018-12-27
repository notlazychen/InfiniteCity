using IfCastle.Interface.Model;
using IfCastle.Interface.ObServers;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Client
{
    public class GameObServer : IAsyncObserver<GameFrameMsg>
    {
        public GameObServer()
        {
        }

        public Task OnCompletedAsync()
        {
            Console.WriteLine("连接完成");
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            Console.WriteLine($"连接错误, ex :{ex}");
            return Task.CompletedTask;
        }

        public Task OnNextAsync(GameFrameMsg item, StreamSequenceToken token = null)
        {
            Console.Clear();
            Console.WriteLine(item.Text);
            return Task.CompletedTask;
        }
    }
}
