namespace Steamerfy.Server.Models
{
    public class Question
    {
        public string QuestionText { get; set; }
        public List<(string,string)> ImageURLAndOption { get; set; } = new();
        public int Answer { get; set; }
        public DateTime ExpireTime { get; set; }

        public Question(string questionText, List<(string, string)> imageURLAndOption, int answer,int Delay)
        {
            QuestionText = questionText;
            ImageURLAndOption = imageURLAndOption;
            Answer = answer;
            ExpireTime = DateTime.Now.AddSeconds(Delay);
        }
    }
}
