using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Models;

namespace Steamerfy.Server.Factory
{
    public class QuestionFactory : IQuestionFactory
    {
        private readonly ISteamHandler _steamHandler;
        public QuestionFactory(ISteamHandler steamHandler) { 
            _steamHandler = steamHandler;
        }
        public async Task<Question> CreateQuestion(Player[] players, int[] askedQuestions)
        {
            Question question = await callSteamApi(players);

            return question;
        }

        public async Task<Question> callSteamApi(Player[] players)
        {
            //call steam api
            return new Question("how are you", "very nice thank you", DateTime.Now);
        }
    }
}
