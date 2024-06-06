namespace Steamerfy.Server.Models
{
    public class SteamItem(string name, string imageUrl, float hoursPlayed, uint TimeLastPlayed)
    {
        public string Name { get; set; } = name;
        public string ImageUrl { get; set; } = imageUrl;
        public float HoursPlayed { get; set; } = hoursPlayed;
        public uint TimeLastPlayed { get; set; } = TimeLastPlayed;
    }
}