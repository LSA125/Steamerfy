using Steamerfy.Server.Models.PlayerDataClasses;

namespace Steamerfy.Server.Models
{
    public class Player(ProfileInfo ProfileInfo, List<SteamItem> steamItems, int Score)
    {
        public string Username { get; set; } = ProfileInfo.Username;
        public string ProfileUrl { get; set; } = ProfileInfo.ProfileUrl;
        public string AvatarUrl { get; set; } = ProfileInfo.AvatarUrl;
        public string SteamId { get; set; } = ProfileInfo.SteamId;
        public string? ConnectionId { get; set; }
        public List<SteamItem> SteamItems { get; set; } = steamItems;
        public int Score { get; set; } = Score;

        public int SelectedAnswer { get; set; } = -1;
    }
}
