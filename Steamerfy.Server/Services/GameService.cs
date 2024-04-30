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
        public Lobby CreateLobby()
        {
            var lobby = new Lobby();
            _lobbies.Add(lobby.Id, lobby);
            return lobby;
        }

        public Lobby GetLobby(int lobbyId)
        {
            return _lobbies.FirstOrDefault(l => l.Key == lobbyId).Value;
        }

        public Player CreatePlayer(Lobby lobby, Player player, string connectionId)
        {
            player.ConnectionId = connectionId;
            lobby.Players.Add(player);
            return player;
        }

        public void AnswerQuestion(Lobby lobby, Player player, int answerId)
        {
            if (lobby.CurrentQuestion != null && lobby.CurrentQuestion.Answer  == answerId)
            {
                player.Score += 1;
            }
        }

        public void NextQuestion(Lobby lobby)
        {
            lobby.CurrentQuestion = _questionFactory.CreateQuestion(lobby.Players);
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
