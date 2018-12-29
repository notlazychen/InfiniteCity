using Fleck;
using IfCastle.Interface;
using IfCastle.Interface.Model;
using Orleans;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Gateway
{
    public class GatewayServer : IDisposable
    {
        public WebSocketServer Server { get; private set; }
        protected ConcurrentDictionary<Guid, IWebSocketConnection> _clients = new ConcurrentDictionary<Guid, IWebSocketConnection>();
        protected IClusterClient _cluster { get; private set; }
        public GatewayServer(string location)
        {
            Server = new WebSocketServer(location);
            Server.ListenerSocket.NoDelay = true;
            Server.RestartAfterListenError = true;
        }

        public void Start()
        {
            _cluster = OrleansLauncher.StartClientWithRetries().Result;

            Server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    _clients.AddOrUpdate(socket.ConnectionInfo.Id, socket, (id, con) => socket);
                    OnSocketOpen(socket);
                };
                socket.OnClose = () =>
                {
                    _clients.TryRemove(socket.ConnectionInfo.Id, out var con);
                    OnSocketClose(socket);
                };
                socket.OnMessage = async message =>
                {
                    try
                    {
                        await OnSocketMessageAsync(socket, message);
                    }
                    catch (Exception)
                    {
                        socket.Close();
                    }
                };
                socket.OnBinary = data => socket.Send(data);
                socket.OnError = (error) =>
                {
                    _clients.TryRemove(socket.ConnectionInfo.Id, out var con);
                    OnSocketError(socket, error);
                };
                socket.OnPing = (data) => socket.SendPong(data);
                //socket.OnPong = (data) => socket.SendPing(data);                
            });
        }

        protected virtual void OnSocketOpen(IWebSocketConnection socket)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{socket.ConnectionInfo.Id}:ENTER");
            Console.ResetColor();
        }

        protected virtual void OnSocketClose(IWebSocketConnection socket)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{socket.ConnectionInfo.Id}:EXIT");
            Console.ResetColor();
        }


        protected virtual async Task OnSocketMessageAsync(IWebSocketConnection socket, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{socket.ConnectionInfo.Id}:{msg}");

            //var guid = Guid.NewGuid();
            var player = _cluster.GetGrain<IPlayer>(socket.ConnectionInfo.Id);
            //var streamId = game.Start().Result;

            string[] ss = msg.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string cmd = ss[0];
            switch (cmd)
            {
                case "enter":
                    {
                        string arg = ss[1];
                        var roomId = Guid.Parse(arg);
                        await player.EnterRoom(roomId);
                        var stream = _cluster.GetStreamProvider(Constants.GameRoomStreamProvider).GetStream<GameFrameMsg>(roomId, Constants.GameRoomStreamNameSpace);
                        await stream.SubscribeAsync(new GatewayObServer(this, player.GetPrimaryKey()));
                    }
                    break;
                case "create":
                    {
                        var roomId = await player.CreateRoom();
                        var stream = _cluster.GetStreamProvider(Constants.GameRoomStreamProvider).GetStream<GameFrameMsg>(roomId, Constants.GameRoomStreamNameSpace);
                        await stream.SubscribeAsync(new GatewayObServer(this, player.GetPrimaryKey()));
                    }
                    break;
                case " ":
                    await player.PauseOrResume();
                    break;
                case "↑":
                    await player.Move(Direction.Up);
                    break;
                case "↓":
                    await player.Move(Direction.Down);
                    break;
                case "←":
                    await player.Move(Direction.Left);
                    break;
                case "→":
                    await player.Move(Direction.Right);
                    break;
            }
            Console.ResetColor();
        }

        protected virtual void OnSocketError(IWebSocketConnection socket, Exception error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{socket.ConnectionInfo.Id}:{error.Message}");
            Console.ResetColor();
        }

        public async Task SendAsync(Guid clientId, string msg)
        {
            if(_clients.TryGetValue(clientId, out var client))
            {
                await client.Send(msg);
            }
        }

        public async Task Broadcast(string msg)
        {
            foreach(var client in _clients.Values)
            {
                await client.Send(msg);
            }
        }

        public void Dispose()
        {
            if (_cluster != null)
                _cluster.Dispose();
            _clients.Clear();
            Server.Dispose();
        }
    }
}
