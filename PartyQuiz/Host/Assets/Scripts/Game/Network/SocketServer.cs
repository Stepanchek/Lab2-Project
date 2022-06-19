using System;
using System.Net.WebSockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Network
{
    public sealed class SocketServer : IDisposable
    {
        private const string HOST = "partyquiz.club";
        private const int PORT = 3013;

        internal bool IsWebSocketCreated { get; private set; }

        private MessageReader _messageReader;
        private MessageWriter _messageWriter;

        internal ClientWebSocket WebSocket { get; private set; }

        [Inject]
        public void Construct(MessageReader messageReader, MessageWriter messageWriter)
        {
            _messageReader = messageReader;
            _messageWriter = messageWriter;
        }

        internal async UniTask Run(string gameId)
        {
            var uri = new Uri($"ws://{HOST}:{PORT}");
            Debug.Log($"Starting a game with id: {gameId}, uri: {uri}");

            await CreateWebSocket(uri);

            _messageReader.Run(gameId);
        }
        
        internal async UniTask Kick(string playerId)
        {
            await _messageWriter.SendObjectAsync(new
            {
                KickPlayer = playerId,
            });
        }

        private async UniTask CreateWebSocket(Uri uri)
        {
            IsWebSocketCreated = false;
            WebSocket?.Dispose();

            WebSocket = new ClientWebSocket();
            await WebSocket.ConnectAsync(uri, CancellationToken.None);

            Debug.Log("Web socket created");

            IsWebSocketCreated = true;
        }
        
        internal async UniTask Stop(string gameId)
        {
            await _messageWriter.SendObjectAsync(new
            {
                EndGame = gameId,
            });
        }

        public void Dispose()
        {
        }
    }
}