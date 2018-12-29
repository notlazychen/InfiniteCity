using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Interface
{
    public interface IPlayer : IGrainWithGuidKey
    {
        Task<Guid> CreateRoom();
        Task EnterRoom(Guid roomId);
        Task CloseRoom();

        Task PauseOrResume();
        Task Restart();

        Task Move(Direction direction);
    }
}
