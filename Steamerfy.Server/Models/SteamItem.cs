namespace Steamerfy.Server.Models
{
    public class SteamItem(string name, string imageUrl, float hoursPlayed, int priceCents, int achievementCount, int achievementTotal, float achievementPercentage)
    {
        public string Name { get; set; } = name;
        public string ImageUrl { get; set; } = imageUrl;
        public float HoursPlayed { get; set; } = hoursPlayed;
        public int PriceCents { get; set; } = priceCents;
        public int AchievementCount { get; set; } = achievementCount;
        public int AchievementTotal { get; set; } = achievementTotal;
        public float AchievementPercentage { get; set; } = achievementPercentage;
    }
}
