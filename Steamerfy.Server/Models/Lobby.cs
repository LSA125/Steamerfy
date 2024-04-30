namespace Steamerfy.Server.Models
{
    public class Lobby
    {
        static readonly Random random = new();
        public int Id { get;} = random.Next(10000000, 99999999);
        public List<Player> Players { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public Question? CurrentQuestion { get; set; } = null;
    }
}