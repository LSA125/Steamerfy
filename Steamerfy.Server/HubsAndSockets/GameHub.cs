using Microsoft.AspNetCore.SignalR;
using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Factory;
using Steamerfy.Server.Models;
using Steamerfy.Server.Services;

namespace Steamerfy.Server.HubsAndSockets
{
    public class GameHub : Hub
    {
        private readonly GameService _gameService;
        private readonly ISteamHandler _steamHandler;
        private const int DELAY_IN_SECONDS = 30;
        public GameHub(GameService gameService, ISteamHandler steamHandler)
        {
            _gameService = gameService;
            _steamHandler = steamHandler;
        }

        public async Task CreateLobby(string hostSteamID)
        {
            int lobbyId = _gameService.CreateLobby(hostSteamID);
            var host = await _steamHandler.GetPlayer(hostSteamID);
            if (host != null)
            {
                host.ConnectionId = Context.ConnectionId;
                _gameService.AddPlayer(_gameService.GetLobby(lobbyId), host);
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
                await Clients.Caller.SendAsync("LobbyCreated", lobbyId);
            }
            else
            {
                await Clients.Caller.SendAsync("PlayerNotFound");
            }
        }

        public async Task JoinLobby(int lobbyId, string steamId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("LobbyNotFound");
                return;
            }
            var player = await _steamHandler.GetPlayer(steamId);
            if (player == null)
            {
                await Clients.Caller.SendAsync("PlayerNotFound");
                return;
            }
            player.ConnectionId = Context.ConnectionId;
            _gameService.AddPlayer(lobby, player);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerJoined", player);
        }

        public async Task AnswerQuestion(int lobbyId, string playerId, int answerId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("LobbyNotFound");
                return;
            }

            var player = lobby.Players.FirstOrDefault(p => p.SteamId == playerId);
            if (player == null)
            {
                await Clients.Caller.SendAsync("PlayerNotFound");
                return;
            }

            _gameService.AnswerQuestion(player, answerId);
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerAnswered", player.Score);
        }

        public async Task StartGameLoop(int lobbyId, string steamId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("LobbyNotFound");
                return;
            }
            if(lobby.HostSteamId != steamId)
            {
                await Clients.Caller.SendAsync("NotHost");
                return;
            }
            _gameService.GenerateQuestion(lobby);
            await Clients.Group(lobbyId.ToString()).SendAsync("QuestionStarted", lobby.CurrentQuestion);
            await Task.Delay(DELAY_IN_SECONDS * 1000);
            await Clients.Group(lobbyId.ToString()).SendAsync("QuestionEnded", lobby.CurrentQuestion);
        }

        public async Task EndGame(int lobbyId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("LobbyNotFound");
                return;
            }

            _gameService.EndGame(lobby);
            await Clients.Group(lobbyId.ToString()).SendAsync("GameEnded", lobby);
        }

        public async Task LeaveLobby(int lobbyId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("LobbyNotFound");
                return;
            }

            var player = lobby.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                await Clients.Caller.SendAsync("PlayerNotFound");
                return;
            }

            _gameService.LeaveLobby(lobby, player);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString(),CancellationToken.None);
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerLeft", player);
        }
    }
}
