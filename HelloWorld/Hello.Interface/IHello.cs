using System;
using System.Threading.Tasks;

namespace Hello.Interface
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}
