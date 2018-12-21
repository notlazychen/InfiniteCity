using IfCastle.Interfaces;
using IfCastle.Interfaces.ObServers;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Grains
{
    public class ChatGrain : Grain<Person>, IChat
    {
        private GrainObserverManager<IChatObserver> _obss = new GrainObserverManager<IChatObserver>();

        public async Task<string> SayHello(string greeting)
        {
            string name = "陌生人";
            if(this.State.Name == null)
            {
                this.State.Name = "第一次遇见你";
                await this.WriteStateAsync();
            }
            else
            {
                name = this.State.Name;
            }
            string msg = $"你好啊, {name}";
            _obss.Notify(client => client.ReceiveMessage(msg));
            return "收到消息";
        }

        public Task Subscribe(IChatObserver chatClient)
        {
            _obss.Subscribe(chatClient);
            return Task.FromResult(0);
        }
    }

    public class Person
    {
        public string Name { get; set; }
    }
}
