namespace Steamerfy.Server.Models.PlayerDataClasses
{
    public class ProfileInfo(string Username, string ProfileUrl, string AvatarUrl, string SteamId, bool IsHost)
    {
        public string Username { get; set; } = Username;
        public string ProfileUrl { get; set; } = ProfileUrl;
        public string AvatarUrl { get; set; } = AvatarUrl;
        public string SteamId { get; set; } = SteamId;
    }
}
