namespace Steamerfy.Server.Models
{
    public class Game
    {
        public int Id { get; set; }
        public required Player[] Players { get; set; }
        //time created
        public DateTime CreatedAt { get; set; }

        public Question currentQuestion { get; set; }
        
    }
}
