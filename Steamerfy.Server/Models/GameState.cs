using Steamerfy.Server.Models.PlayerDataClasses;

namespace Steamerfy.Server.Models
{
    public class GameState(int LobbyId, Question? CurrentQuestion, List<ProfileInfo> Players,List<List<string>> AnswerData, string HostSteamId)
    {
        public int LobbyId { get; set; } = LobbyId;
        public Question? CurrentQuestion { get; set; } = CurrentQuestion;
        public List<ProfileInfo> Players { get; set; } = Players;
        public List<List<string>> AnswerData { get; set; } = AnswerData;
        public string HostSteamId { get; set; } = HostSteamId;
    }
}
