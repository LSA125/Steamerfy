namespace Steamerfy.Server.Models
{
    public class Game
    {
        public int Id { get; set; }
        public required Player[] Players { get; set; }
        public DateTime CreatedAt { get; set; }
        public Question? CurrentQuestion { get; set; }
    }
}