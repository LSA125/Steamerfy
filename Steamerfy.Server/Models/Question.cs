namespace Steamerfy.Server.Models
{
    public class Question
    {
        public string QuestionText { get; set; }
        public string QuestionURL { get; set; }
        public List<List<string>> ImageURLAndOption { get; set; } = [];
        public int Answer { get; set; }
        public DateTime ExpireTime { get; set; }
        public Question(string questionText, string questionURL, List<List<string>> imageURLAndOption, int answer,int Delay)
        {
            QuestionText = questionText;
            QuestionURL = questionURL;
            ImageURLAndOption = imageURLAndOption;
            Answer = answer;
            ExpireTime = DateTime.Now.AddSeconds(Delay);
        }
    }
}
