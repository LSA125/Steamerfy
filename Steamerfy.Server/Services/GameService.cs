using Steamerfy.Server.Factory;
using Steamerfy.Server.Models;

namespace Steamerfy.Server.Services
{
    public class GameService
    {
        private readonly Dictionary<int, Lobby> _lobbies = [];
        private readonly Dictionary<string, Lobby> _lobbiesByConnectionId = [];
        private readonly IQuestionFactory _questionFactory;

        public GameService(IQuestionFactory questionFactory)
        {
            _questionFactory = questionFactory;
        }
        public int CreateLobby(string hostSteamId, uint maxScore)
        {
            if (String.IsNullOrEmpty(hostSteamId))
            {
                throw new ArgumentNullException(nameof(hostSteamId));
            }

            var lobby = new Lobby(hostSteamId,maxScore);
            _lobbies.Add(lobby.Id, lobby);
            return lobby.Id;
        }
        public void SetHost(int lobbyId, string hostSteamId)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Key == lobbyId).Value;
            lobby.HostSteamId = hostSteamId;
        }

        public Lobby GetLobby(int lobbyId)
        {
            return _lobbies.FirstOrDefault(l => l.Key == lobbyId).Value;
        }
        public Lobby GetLobbyByConnectionId(string connectionId)
        {
            return _lobbiesByConnectionId.FirstOrDefault(l => l.Key == connectionId).Value;
        }
        public List<(string,int)> GetPlayerScores(Lobby lobby)
        {
            return lobby.Players.Select(p => (p.SteamId, p.Score)).ToList();
        }

        public Player AddPlayer(Lobby lobby, Player player)
        {
            if (player.ConnectionId == null)
            {
                throw new ArgumentNullException(nameof(player.ConnectionId));
            }
            lobby.Players.Add(player);
            _lobbiesByConnectionId.Add(player.ConnectionId, lobby);
            return player;
        }

        public void AnswerQuestion(Player player, int answerId)
        {
            player.SelectedAnswer = answerId;
        }

        public void GenerateQuestion(Lobby lobby)
        {
            lobby.CurrentQuestion = _questionFactory.CreateQuestion(lobby.Players);
        }

        public Question? GetQuestion(Lobby lobby)
        {
            return lobby.CurrentQuestion;
        }
        //returns true if some players have answered
        public void UpdateAndPrepareScores(Lobby lobby)
        {
            foreach (var player in lobby.Players)
            {
                if (player.SelectedAnswer == lobby.CurrentQuestion?.Answer)
                {
                    player.Score += 1;
                }
            }
        }

        public void ResetAnswers(Lobby lobby)
        {
            foreach (var player in lobby.Players)
            {
                player.SelectedAnswer = -1;
            }
        }
        public List<List<string>> GetAnswerData(Lobby lobby)
        {
            return lobby.Players.Select(p => new List<string> { p.SteamId, p.SelectedAnswer.ToString(), p.Score.ToString() }).ToList();
        }

        public void EndGame(Lobby lobby)
        {
            _lobbies.Remove(lobby.Id);
        }

        public void RemovePlayerConnections(Player player)
        {
            if(player.ConnectionId == null)
            {
                throw new ArgumentNullException(nameof(player.ConnectionId));
            }
            _lobbiesByConnectionId.Remove(player.ConnectionId);
        }
    }
}
