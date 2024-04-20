using Steamerfy.Server.Models;

namespace Steamerfy.Server.Factory
{
    public interface IQuestionFactory
    {
        public  Task<Question> CreateQuestion(Player[] players, int[] askedQuestions);
    }
}
