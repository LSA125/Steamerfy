namespace Steamerfy.Server.Models.PlayerDataClasses
{
    public class PlayerState(string Username, string ProfileUrl, string AvatarUrl, string SteamId,int Score)
    {
        public string Username { get; set; } = Username;
        public string ProfileUrl { get; set; } = ProfileUrl;
        public string AvatarUrl { get; set; } = AvatarUrl;
        public string SteamId { get; set; } = SteamId;
        public int Score { get; set; } = Score;
    }
}
