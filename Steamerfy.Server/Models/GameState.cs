namespace Steamerfy.Server.Models
{
    public class GameState(int LobbyId, Question? CurrentQuestion, List<Player> Players, string HostSteamId)
    {
        public int LobbyId { get; set; } = LobbyId;
        public Question? CurrentQuestion { get; set; } = CurrentQuestion;
        public List<Player> Players { get; set; } = Players;
        public string HostSteamId { get; set; } = HostSteamId;
    }
}
