using IfCastle.Interface.ObServers;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Interface
{
    public interface IBlockGame : IGrainWithGuidKey
    {
        Task Start();

        Task Move(Direction direction);

        Task Pause();

        Task Resume();

        //Task Subscribe(IChatObserver chatClient);
    }

    public enum Direction
    {
        Up, Down, Left, Right
    }
}
