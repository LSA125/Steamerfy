using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Models;

namespace Steamerfy.Server.Factory
{
    public class QuestionFactory : IQuestionFactory
    {
        public  Question CreateQuestion(List<Player> players)
        {
            Question question = new Question("What is the capital of France?", "https://preview.redd.it/ceetrhas51441.jpg?auto=webp&s=84ab80b5034f99e055f4105baa18ef7e7e0914e0",
                                    [["https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "Paris"], ["https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "London"], ["https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "Berlin"], ["https://i.kym-cdn.com/photos/images/newsfeed/002/444/001/a3e.jpg", "Madrid"]],
                                    0,15);

            return question;
        }
    }
}
