using System;
using System.Threading.Tasks;
using Hello.Interface;

namespace Hello.Grain
{
    public class HelloGrain : Orleans.Grain, IHello
    {
        public Task<string> SayHello(string greeting)
        {
            return Task.FromResult($"You said: '{greeting}', I say: Hello!");
        }
    }
}
