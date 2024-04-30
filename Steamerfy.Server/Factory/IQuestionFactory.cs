using Steamerfy.Server.Models;

namespace Steamerfy.Server.Factory
{
    public interface IQuestionFactory
    {
        public Question CreateQuestion(List<Player> players);
    }
}
