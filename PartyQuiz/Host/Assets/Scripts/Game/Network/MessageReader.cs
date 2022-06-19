using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PartyQuiz.Gameplay;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Network
{
    public sealed class MessageReader
    {
        private GameController _gameController;
        private SocketServer _socketServer;
        private MessageWriter _messageWriter;

        private string _gameId;

        [Inject]
        public void Construct(GameController gameController, SocketServer socketServer, MessageWriter messageWriter)
        {
            _gameController = gameController;
            _socketServer = socketServer;
            _messageWriter = messageWriter;
        }

        internal void Run(string gameId)
        {
            _gameId = gameId;

            MainLoop().HandleExceptions();
        }

        private async UniTask MainLoop()
        {
            using (_socketServer.WebSocket)
            {
                await _messageWriter.SendObjectAsync(new
                {
                    StartGame = _gameId,
                });

                while (_socketServer.WebSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        var matchmakerMessage = await ReadStringAsync();

                        Debug.Log($"in: {matchmakerMessage}");

                        var response = JsonConvert.DeserializeObject<Response>(matchmakerMessage);

                        if (response == null)
                            throw new IOException("Response came in null");

                        if (response.CurrentState != null)
                            OnInit(response.CurrentState);
                        else if (!string.IsNullOrEmpty(response.PlayerJoined))
                            OnPlayerJoined(response.PlayerJoined);
                        else if (!string.IsNullOrEmpty(response.PlayerLeft))
                            OnPlayerLeft(response.PlayerLeft);
                        else if (!string.IsNullOrEmpty(response.PlayerConnected))
                            OnPlayerConnected(response.PlayerConnected);
                        else if (!string.IsNullOrEmpty(response.PlayerDisconnected))
                            OnPlayerDisconnected(response.PlayerDisconnected);
                        else if (response.PlayerMessage != null)
                            OnPlayerMessage(response.PlayerMessage.id, response.PlayerMessage.text);
                    }
                    catch (WebSocketException socketException)
                    {
                        Debug.LogError($"Matchmaker has been disconnected: {socketException.Message}");
                        _gameController.TryToReconnect();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        await new WaitForSeconds(3.0f);
                    }
                }
            }
        }

        private async UniTask<string> ReadStringAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);

            using var ms = new MemoryStream();

            WebSocketReceiveResult result;

            do
            {
                result = await _socketServer.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array ?? Array.Empty<byte>(), buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(ms, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        public Player CreatePlayer(string id, bool isOnline)
        {
            return new Player(id, isOnline, _messageWriter, _gameController);
        }

        /// <summary>
        /// Init status of already joined/registered players
        /// </summary>
        private void OnInit(PlayerState[] playerStates)
        {
            foreach (var playerState in playerStates)
            {
                var player = CreatePlayer(playerState.id, playerState.online);
                _gameController.AddPlayer(player);
            }
        }

        /// <summary>
        /// New player joined the game
        /// </summary>
        private void OnPlayerJoined(string playerId)
        {
            var player = CreatePlayer(playerId, true);
            _gameController.AddPlayer(player);
        }

        /// <summary>
        /// Player left the game
        /// </summary>
        private void OnPlayerLeft(string playerId)
        {
            _gameController.RemovePlayer(playerId);
        }

        /// <summary>
        /// Player connected back (reconnected)
        /// </summary>
        private void OnPlayerConnected(string playerId)
        {
            _gameController.NotifyPlayerConnected(playerId);
        }

        /// <summary>
        /// Player left the game temporarily (closed the browser, etc)
        /// </summary>
        private void OnPlayerDisconnected(string playerId)
        {
            _gameController.NotifyPlayerDisconnected(playerId);
        }

        private void OnPlayerMessage(string targetPlayerId, string message)
        {
            var innerMessage = message.Replace("\\\"", "\"");
            Debug.Log("[Inner] message: " + innerMessage);

            _gameController.PlayerGotMessage(targetPlayerId, message);
        }
    }
}