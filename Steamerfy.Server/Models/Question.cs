namespace Steamerfy.Server.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public DateTime ExpireTime { get; set; }
        public Question(string questionText, string answer, DateTime expireTime)
        {
            QuestionText = questionText;
            Answer = answer;
            ExpireTime = expireTime;
        }
    }
}
