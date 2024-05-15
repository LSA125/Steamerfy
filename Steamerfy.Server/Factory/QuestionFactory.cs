using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Models;

namespace Steamerfy.Server.Factory
{
    public class QuestionFactory : IQuestionFactory
    {
        public  Question CreateQuestion(List<Player> players)
        {
            Question question = new Question("What is the capital of France?", [("https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "Paris"), ("https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "London"), ("https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "Berlin"), ("https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "Madrid")], 0,15);

            return question;
        }
    }
}
