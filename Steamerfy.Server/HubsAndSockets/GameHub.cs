using Microsoft.AspNetCore.SignalR;
using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Factory;
using Steamerfy.Server.Services;

namespace Steamerfy.Server.HubsAndSockets
{
    public class GameHub : Hub
    {
        private readonly GameService _gameService;
        private readonly ISteamHandler _steamHandler;
        private readonly IQuestionFactory _questionFactory;
        public GameHub(GameService gameService, ISteamHandler steamHandler)
        {
            _gameService = gameService;
            _steamHandler = steamHandler;
        }

        public async Task CreateLobby()
        {
            var lobby = _gameService.CreateLobby();
            await Clients.Caller.SendAsync("LobbyCreated", lobby);
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
            var connectionId = Context.ConnectionId;
            _gameService.CreatePlayer(lobby, player, connectionId);
            await Groups.AddToGroupAsync(connectionId, lobbyId.ToString());
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerJoined", player);
        }

        public async Task StartGame(int lobbyId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("LobbyNotFound");
                return;
            }
            await Clients.Group(lobbyId.ToString()).SendAsync("GameStarted", lobby);
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

            _gameService.AnswerQuestion(lobby, player, answerId);
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerAnswered", player.Score);
        }

        public async Task NextQuestion(int lobbyId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("LobbyNotFound");
                return;
            }

            _gameService.NextQuestion(lobby);
            await Clients.Group(lobbyId.ToString()).SendAsync("QuestionStarted", lobby.CurrentQuestion);
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
