using IfCastle.Interface.Model;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Gateway
{
    public class GatewayObServer : IAsyncObserver<GameFrameMsg>
    {
        private GatewayServer _server;
        private Guid _socketId;

        public GatewayObServer(GatewayServer server, Guid socketId)
        {
            _socketId = socketId;
            _server = server;
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

        public async Task OnNextAsync(GameFrameMsg item, StreamSequenceToken token = null)
        {
            //Console.Clear();
            await _server.SendAsync(_socketId, item.Text);
            //Console.WriteLine(item.Text);
        }
    }
}
