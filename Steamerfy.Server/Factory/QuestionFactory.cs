using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Models;

namespace Steamerfy.Server.Factory
{
    public class QuestionGenerator : IQuestionFactory
    {
        private readonly Random random = new();

        public Question CreateQuestion(List<Player> players)
        {
            // Define question types
            var questionTypes = new List<Func<List<Player>, Question>>
            {
                GenerateMostHoursQuestion,
                //GenerateMostRecentlyPlayedQuestion,
                GenerateMostPlayedGameQuestion,
                //GenerateMostRecentlyPlayedSingle
            };

            if (players.Count < 2)
            {
                questionTypes =
                [
                    GenerateMostPlayedGameQuestion,
                    //GenerateMostRecentlyPlayedSingle
                ];
            }

            // Pick a random question type
            var questionType = questionTypes[random.Next(questionTypes.Count)];

            // Generate the question
            return questionType(players);
        }

        private List<T> GetRandomElements<T>(List<T> list, int count)
        {
            return list.OrderBy(x => random.Next()).Take(count).ToList();
        }

        // Selects a random game that at least 2 players have played, and asks which player has played it the most
        private Question GenerateMostHoursQuestion(List<Player> players)
        {
            var gamesPlayedByMultiple = players
                .SelectMany(p => p.SteamItems)
                .Where(i => i.HoursPlayed > 0)
                .GroupBy(game => game.Name)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (gamesPlayedByMultiple.Count == 0)
                return new Question("Y'all' dont share any games. SHAME", "", [], -1, 5);

            var game = gamesPlayedByMultiple[random.Next(gamesPlayedByMultiple.Count)];

            var relevantPlayers = players
                .Where(p => p.SteamItems.Any(i => i.Name == game))
                .ToList();

            relevantPlayers = GetRandomElements(relevantPlayers, Math.Min(relevantPlayers.Count, 4));

            var mostHoursPlayer = relevantPlayers
                .OrderByDescending(p => p.SteamItems.First(i => i.Name == game).HoursPlayed)
                .First();
            var mostHours = mostHoursPlayer.SteamItems.First(i => i.Name == game).HoursPlayed;
            var options = relevantPlayers.Select(p => new List<string> { p.AvatarUrl, p.Username }).ToList();

            options = [.. options.OrderBy(x => random.Next())];

            if (!options.Any(o => o[1] == mostHoursPlayer.Username))
            {
                options[random.Next(options.Count)] = [mostHoursPlayer.AvatarUrl, mostHoursPlayer.Username];
            }

            var questionText = $"Who has the most hours on {game}?";
            var answerIndex = options.FindIndex(o => o[1] == mostHoursPlayer.Username);
            return new Question(questionText,"", options, answerIndex,15);
        }

        // Selects a random game that at least 2 players have played, and asks which player has played it most recently
        private Question GenerateMostRecentlyPlayedQuestion(List<Player> players)
        {
            var gamesPlayedByMultiple = players
                .SelectMany(p => p.SteamItems)
                .Where(i => i.HoursPlayed > 0)
                .GroupBy(game => game.Name)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (gamesPlayedByMultiple.Count == 0)
                return new Question("Y'all' dont share any games. SHAME", "", [], -1, 5);

            var game = gamesPlayedByMultiple[random.Next(gamesPlayedByMultiple.Count)];

            var relevantPlayers = players
                .Where(p => p.SteamItems.Any(i => i.Name == game))
                .ToList();

            relevantPlayers = GetRandomElements(relevantPlayers, Math.Min(relevantPlayers.Count, 4));

            var mostRecentlyPlayedPlayer = relevantPlayers
                .OrderBy(p => p.SteamItems.First(i => i.Name == game).TimeLastPlayed)
                .First();

            var timelastplayed = mostRecentlyPlayedPlayer.SteamItems.First(i => i.Name == game).TimeLastPlayed;

            var options = relevantPlayers.Select(p => new List<string> { p.AvatarUrl, p.Username }).ToList();


            options = options.OrderBy(x => random.Next()).ToList();

            if (!options.Any(o => o[1] == mostRecentlyPlayedPlayer.Username))
            {
                options[random.Next(options.Count)] = [mostRecentlyPlayedPlayer.AvatarUrl, mostRecentlyPlayedPlayer.Username];
            }

            var questionText = $"Who most recently played {game}? ({timelastplayed/60/24} days ago)";
            var answerIndex = options.FindIndex(o => o[1] == mostRecentlyPlayedPlayer.Username);
            return new Question(questionText, "", options, answerIndex, 15);
        }

        // Selects a random player, and 4 of their games, and asks which game they have played the most
        private Question GenerateMostPlayedGameQuestion(List<Player> players)
        {
            var player = players[random.Next(players.Count)];
            var steamItems = GetRandomElements(player.SteamItems, 4);

            if (steamItems.Count < 4)
                return new Question(player.Username + " does not own 4 steam games. SHAME", "", [], -1, 5);

            var mostPlayedGame = steamItems.OrderBy(i => i.HoursPlayed).Last();

            var options = steamItems.Select(i => new List<string> { i.ImageUrl, i.Name }).ToList();


            options = [.. options.OrderBy(x => random.Next())];

            if (!options.Any(o => o[1] == mostPlayedGame.Name))
            {
                options[random.Next(options.Count)] = [mostPlayedGame.ImageUrl, mostPlayedGame.Name];
            }

            var questionText = $"What game has {player.Username} played the most? ({mostPlayedGame.HoursPlayed} hours)";
            var answerIndex = options.FindIndex(o => o[1] == mostPlayedGame.Name);
            return new Question(questionText, player.AvatarUrl, options, answerIndex, 15);
        }

        // Selects a random player, and 4 of their games, and asks which game they played most recently
        private Question GenerateMostRecentlyPlayedSingle(List<Player> players)
        {
            var player = players[random.Next(players.Count)];
            var steamItems = GetRandomElements(player.SteamItems, 4);

            if (steamItems.Count < 4)
                return new Question(player.Username + " does not own 4 steam games. SHAME", "", [], -1, 5);

            var mostRecentlyPlayedGame = steamItems.OrderBy(i => i.TimeLastPlayed).First();

            var options = steamItems.Select(i => new List<string> { i.ImageUrl, i.Name }).ToList();

            options = [.. options.OrderBy(x => random.Next())];

            if (!options.Any(o => o[1] == mostRecentlyPlayedGame.Name))
            {
                options[random.Next(options.Count)] = [mostRecentlyPlayedGame.ImageUrl, mostRecentlyPlayedGame.Name];
            }

            var questionText = $"Which game has {player.Username} played most recently? ({mostRecentlyPlayedGame.TimeLastPlayed/60/24} days ago)";
            var answerIndex = options.FindIndex(o => o[1] == mostRecentlyPlayedGame.Name);
            //remove duplicate for the same hours
            return new Question(questionText, player.AvatarUrl, options, answerIndex, 10);
        }
    }
}
