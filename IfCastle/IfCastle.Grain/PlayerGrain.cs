using IfCastle.Grain.States;
using IfCastle.Interface;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Grain
{
    public class PlayerGrain : Grain<PlayerState>, IPlayer
    {
        public override Task OnActivateAsync()
        {
            this.State.Id = this.GetPrimaryKey();
            return base.OnActivateAsync();
        }

        public async Task CloseRoom()
        {
            if (this.State.RoomId != default(Guid))
            {
                var room = GrainFactory.GetGrain<IBlockGame>(this.State.RoomId);
                await room.Close();
            }
        }

        public async Task<Guid> CreateRoom()
        {
            if (this.State.RoomId == default(Guid))
            {
                var room = GrainFactory.GetGrain<IBlockGame>(Guid.NewGuid());
                await room.Start();
                this.State.RoomId = room.GetPrimaryKey();
            }
            return this.State.RoomId;
        }

        public Task EnterRoom(Guid roomId)
        {
            if(this.State.RoomId == default(Guid))
            {
                var room = GrainFactory.GetGrain<IBlockGame>(roomId);
                this.State.RoomId = roomId;
            }
            return Task.CompletedTask;
        }

        public async Task Move(Direction direction)
        {
            if (this.State.RoomId == default(Guid))
            {
                return;
            }
            var room = GrainFactory.GetGrain<IBlockGame>(this.State.RoomId);
            await room.Move(direction);
        }

        public async Task PauseOrResume()
        {
            if (this.State.RoomId == default(Guid))
            {
                return;
            }
            var room = GrainFactory.GetGrain<IBlockGame>(this.State.RoomId);
            await room.PauseOrResume();
        }

        public async Task Restart()
        {
            if (this.State.RoomId == default(Guid))
            {
                return;
            }
            var room = GrainFactory.GetGrain<IBlockGame>(this.State.RoomId);
            await room.Restart();
        }
    }
}
