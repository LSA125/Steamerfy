namespace Steamerfy.Server.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? ProfileUrl { get; set; }
        public string? AvatarUrl { get; set; }
        public string? SteamId { get; set; }
        public int Score { get; set; }
        public int TotalCentsSpent { get; set; }
        public int TotalHoursPlayed { get; set; }
        public int TotalAchievements { get; set; }
        public SteamItem[]? SteamItems { get; set; }
    }
}
