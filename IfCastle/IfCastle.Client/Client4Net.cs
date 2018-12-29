using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;
namespace IfCastle.Client
{
    public class Client4Net : IClient
    {
        private WebSocket _ws;
        //private readonly byte[] _buffer;
        //private int _offset = 0;

        /// <summary>
        /// 网络延迟
        /// </summary>
        public int NetDelay { get; private set; }
        private Queue<long> _delays = new Queue<long>();
        private Timer _timer;
        public Client4Net()
        {
            //_buffer = new byte[bufferSize]; 
        }

        public async Task ConnectAsync(string uri)
        {
            _ws = new WebSocket(uri);
            _ws.NoDelay = true;
            //_ws.EnableAutoSendPing = true;
            //_ws.AutoSendPingInterval = 1;

            _ws.MessageReceived += (o,e)=> OnMessage(e.Message);
            _ws.DataReceived += (o,e)=> OnPong(e.Data);
            _ws.Error += _ws_Error;
            _ws.Open();

            _timer = new Timer(Ping, null, 1000, 1000);            
        }

        private void Ping(object state)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var data = BitConverter.GetBytes(now);
            _ws.Send(data, 0, data.Length);

            Console.SetCursorPosition(0, 8);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"延迟: {this.NetDelay}");
            Console.ResetColor();
        }

        private void OnPong(byte[] data)
        {
            var lt = BitConverter.ToInt64(data);
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var delta = now - lt;
            _delays.Enqueue(delta);
            NetDelay = (int)_delays.Average();
            if(_delays.Count > 5)
            {
                _delays.Dequeue();
            }
        }

        private void _ws_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {e.Exception.Message}");
            Console.ResetColor();
        }

        private async Task ReceiveMsgAsync()
        {
            await ReceiveMsgAsync();
        }

        public async Task SendAsync(string msg)
        {
            _ws.Send(msg);
        }

        protected virtual void OnMessage(string msg)
        {
            Console.SetCursorPosition(0, 9);
            Console.WriteLine(msg);
            //Console.SetCursorPosition(0, 17);
        }

        public void Dispose()
        {
            _ws.Dispose();
        }
    }
}
