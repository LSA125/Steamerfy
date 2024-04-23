using Steamerfy.Server.Models.PlayerDataClasses;

namespace Steamerfy.Server.Models
{
    public class Player(ProfileInfo ProfileInfo, GameInfo GameInfo, int Score)
    {
        public int Id { get; set; }
        public ProfileInfo ProfileInfo { get; set; } = ProfileInfo;
        public GameInfo GameInfo { get; set; } = GameInfo;
        public int Score { get; set; } = Score;
    }
}
