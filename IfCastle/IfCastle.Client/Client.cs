using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IfCastle.Client
{
    public class Client : IClient
    {
        private readonly ClientWebSocket _ws = new ClientWebSocket();
        private readonly byte[] _buffer;
        private int _offset = 0;

        /// <summary>
        /// 网络延迟
        /// </summary>
        public int NetDelay { get; private set; }

        public Client(int bufferSize = 10240)
        {
            _buffer = new byte[bufferSize];
        }

        public async Task ConnectAsync(string uri)
        {
            await _ws.ConnectAsync(new Uri(uri), CancellationToken.None);
            ReceiveMsgAsync();
        }

        private async Task ReceiveMsgAsync()
        {
            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(_buffer, _offset, _buffer.Length - _offset);
            WebSocketReceiveResult result = await _ws.ReceiveAsync(bytesReceived, CancellationToken.None);
            _offset += result.Count;
            if (result.EndOfMessage)
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        var msg = Encoding.UTF8.GetString(_buffer, 0, _offset);
                        OnMessage(msg);
                        break;
                }
                _offset = 0;
            }
            await ReceiveMsgAsync();
        }

        public async Task SendAsync(string msg)
        {
            await _ws.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);
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
