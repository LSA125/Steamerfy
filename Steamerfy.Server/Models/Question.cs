namespace Steamerfy.Server.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public DateTime expireTime { get; set; }
    }
}
