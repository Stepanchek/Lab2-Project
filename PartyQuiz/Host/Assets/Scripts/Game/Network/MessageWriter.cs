using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Network
{
    public sealed class MessageWriter
    {
        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private SocketServer _socketServer;

        [Inject]
        public void Construct(SocketServer socketServer)
        {
            _socketServer = socketServer;
        }
        
        internal UniTask SendObjectWithTargetPlayer(string id, object message)
        {
            return SendMessageWithTargetPlayer(id, JsonConvert.SerializeObject(message));
        }

        internal UniTask SendError(string id, string message)
        {
            return SendObjectWithTargetPlayer(id, new
            {
                Command = "Error",
                Message = message
            });
        }

        private UniTask SendMessageWithTargetPlayer(string id, string message)
        {
            Debug.Log($"out: to player: {id}, message: {message}");
            
            return SendObjectAsync(new
            {
                Message = new
                {
                    target = new { Id = id },
                    message
                }
            });
        }
        
        internal UniTask SendObjectAsync(object obj)
        {
            var text = JsonConvert.SerializeObject(obj);
            return SendStringAsync(text);
        }

        private async UniTask SendStringAsync(string text)
        {
            await new WaitUntil(() => _socketServer.IsWebSocketCreated);

            await _sendLock.WaitAsync();

            try
            {
                await _socketServer.WebSocket.SendAsync(
                    buffer: new ArraySegment<byte>(Encoding.UTF8.GetBytes(text)),
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    CancellationToken.None);
            }
            finally
            {
                _sendLock.Release();
            }
        }
    }
}