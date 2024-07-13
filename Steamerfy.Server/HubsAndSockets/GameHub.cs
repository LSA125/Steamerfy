using Microsoft.AspNetCore.SignalR;
using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Models;
using Steamerfy.Server.Models.PlayerDataClasses;
using Steamerfy.Server.Services;

namespace Steamerfy.Server.HubsAndSockets
{
    public class GameHub : Hub
    {
        private readonly GameService _gameService;
        private readonly ISteamHandler _steamHandler;
        public GameHub(GameService gameService, ISteamHandler steamHandler)
        {
            _gameService = gameService;
            _steamHandler = steamHandler;
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var lobby = _gameService.GetLobbyByConnectionId(Context.ConnectionId);
            if (lobby != null)
            {
                var player = lobby.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
                if (player != null)
                {
                    if (lobby.HostSteamId == player.SteamId)
                    {
                        var tmp = lobby.Players.FirstOrDefault(p => p.SteamId != player.SteamId)?.SteamId;
                        if(tmp != null)
                        {
                            lobby.HostSteamId = tmp;
                            await Clients.Group(lobby.Id.ToString()).SendAsync("HostChanged", lobby.HostSteamId);
                        }
                        else
                        {
                            DeleteLobbyAndRemoveGroup(lobby);
                            return;
                        }
                    }
                    _gameService.RemovePlayerConnections(player);
                    lobby.Players.Remove(player);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobby.Id.ToString(), CancellationToken.None);
                    await Clients.Group(lobby.Id.ToString()).SendAsync("PlayerLeft", player);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateLobby(string hostSteamID, uint maxScore)
        {
            if(maxScore <= 1 || maxScore > 14)
            {
                await Clients.Caller.SendAsync("error", "Invalid max score");
                return;
            }
            int lobbyId = _gameService.CreateLobby(hostSteamID,maxScore);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
            await Clients.Caller.SendAsync("LobbyCreated", lobbyId);
        }

        public async Task JoinLobby(int lobbyId, string steamId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("InvalidLobby");
                return;
            }
            var player = await _steamHandler.GetPlayer(steamId);
            if (player == null)
            {
                await Clients.Caller.SendAsync("error","PlayerNotFound");
                if(lobby.Players.Count == 0)
                {
                    DeleteLobbyAndRemoveGroup(lobby);
                }
                return;
            }

            player.ConnectionId = Context.ConnectionId;
            var profiles = lobby.Players.Select(p => new PlayerState(p.Username, p.ProfileUrl, p.AvatarUrl, p.SteamId,p.Score)).ToList();
            await Clients.Caller.SendAsync("LobbyJoined", new GameState(lobbyId, lobby.CurrentQuestion,profiles,_gameService.GetAnswerData(lobby),lobby.HostSteamId));
            if(lobby.Players.TrueForAll(p => p.SteamId != player.SteamId))
            {
                _gameService.AddPlayer(lobby, player);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
            var profileInfo = new ProfileInfo(player.Username, player.ProfileUrl, player.AvatarUrl, player.SteamId);
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerJoined", profileInfo);
        }

        public async Task AnswerQuestion(int lobbyId, string playerId, int answerId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("error","Lobby Not Found");
                return;
            }

            var player = lobby.Players.FirstOrDefault(p => p.SteamId == playerId);
            if (player == null)
            {
                await Clients.Caller.SendAsync("error", "Player Not Found");
                return;
            }

            _gameService.AnswerQuestion(player, answerId);
            //if all players have answered
            if (lobby.Players.All(p => p.SelectedAnswer != -1))
            {
                _gameService.UpdateAndPrepareScores(lobby);
                await Clients.Group(lobbyId.ToString()).SendAsync("QuestionEnded", _gameService.GetAnswerData(lobby));
                _gameService.ResetAnswers(lobby);
            }
        }

        public async Task StartQuestion(int lobbyId, string steamId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("error", "Lobby Not Found");
                return;
            }
            if(lobby.HostSteamId != steamId)
            {
                await Clients.Caller.SendAsync("error", "You are not the host");
                return;
            }
            if(lobby.Players.Any(player => player.Score >= lobby.MaxScore))
            {
                await Clients.Group(lobbyId.ToString()).SendAsync("GameEnd");
            }
            _gameService.GenerateQuestion(lobby);
            await Clients.Group(lobbyId.ToString()).SendAsync("QuestionStarted", lobby.CurrentQuestion);
        }

        public async Task EndQuestion(int lobbyId, string steamId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("error", "Lobby Not Found");
                return;
            }
            if (lobby.HostSteamId != steamId)
            {
                await Clients.Caller.SendAsync("error", "You are not the host");
                return;
            }
           _gameService.UpdateAndPrepareScores(lobby);
            await Clients.Group(lobbyId.ToString()).SendAsync("QuestionEnded", _gameService.GetAnswerData(lobby));
            _gameService.ResetAnswers(lobby);
        }

        public async Task EndGame(int lobbyId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("error", "Lobby Not Found");
                return;
            }

            _gameService.EndGame(lobby);
            await Clients.Group(lobbyId.ToString()).SendAsync("GameEnded", _gameService.GetAnswerData(lobby));
        }

        public async Task LeaveLobby(int lobbyId)
        {
            var lobby = _gameService.GetLobby(lobbyId);
            if (lobby == null)
            {
                await Clients.Caller.SendAsync("error", "Lobby Not Found");
                return;
            }

            var player = lobby.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                await Clients.Caller.SendAsync("error", "Player Not Found");
                return;
            }

            _gameService.RemovePlayerConnections(player);
            lobby.Players.Remove(player);
            if (lobby.Players.Count == 0)
            {
                DeleteLobbyAndRemoveGroup(lobby);
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString(),CancellationToken.None);
            await Clients.Group(lobbyId.ToString()).SendAsync("PlayerLeft", player);
        }

        private async void DeleteLobbyAndRemoveGroup(Lobby lobby)
        {
            foreach(var player in lobby.Players)
            {
                if (player.ConnectionId != null)
                {
                    await Groups.RemoveFromGroupAsync(player.ConnectionId, lobby.Id.ToString(), CancellationToken.None);
                }
                _gameService.RemovePlayerConnections(player);
            }
            lobby.Players.Clear();
            _gameService.EndGame(lobby);
        }
    }
}
