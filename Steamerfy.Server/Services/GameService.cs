using Steamerfy.Server.Factory;
using Steamerfy.Server.Models;

namespace Steamerfy.Server.Services
{
    public class GameService
    {
        private readonly Dictionary<int, Lobby> _lobbies = [];
        private readonly IQuestionFactory _questionFactory;

        public GameService(IQuestionFactory questionFactory)
        {
            _questionFactory = questionFactory;
        }
        public int CreateLobby(string hostSteamId)
        {
            if (String.IsNullOrEmpty(hostSteamId))
            {
                throw new ArgumentNullException(nameof(hostSteamId));
            }

            var lobby = new Lobby(hostSteamId);
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

        public List<(string,int)> GetPlayerScores(Lobby lobby)
        {
            return lobby.Players.Select(p => (p.SteamId, p.Score)).ToList();
        }

        public Player AddPlayer(Lobby lobby, Player player)
        {
            lobby.Players.Add(player);
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

        public void EndGame(Lobby lobby)
        {
            _lobbies.Remove(lobby.Id);
        }

        public void LeaveLobby(Lobby lobby, Player player)
        {
            lobby.Players.Remove(player);
        }
    }
}
