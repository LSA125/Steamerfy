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
                GenerateMostRecentlyPlayedQuestion,
                GenerateMostPlayedGameQuestion,
                GenerateMostRecentlyPlayedSingle
            };

            if (players.Count < 2)
            {
                questionTypes =
                [
                    GenerateMostPlayedGameQuestion,
                    GenerateMostRecentlyPlayedSingle
                ];
            }

            // Pick a random question type
            var questionType = questionTypes[random.Next(questionTypes.Count)];

            // Generate the question
            return questionType(players);
        }

        // Selects a random game that at least 2 players have played, and asks which player has played it the most
        private Question GenerateMostHoursQuestion(List<Player> players)
        {
            var gamesPlayedByMultiple = players
                .SelectMany(p => p.SteamItems)
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

            var mostHoursPlayer = relevantPlayers
                .OrderByDescending(p => p.SteamItems.First(i => i.Name == game).HoursPlayed)
                .First();
            var mostHours = mostHoursPlayer.SteamItems.First(i => i.Name == game).HoursPlayed;
            var options = relevantPlayers.Select(p => new List<string> { p.AvatarUrl, p.Username }).ToList();

            options = [.. options.OrderBy(x => random.Next())];

            var answerIndex = options.FindIndex(o => o[1] == mostHoursPlayer.Username);

            if (!options.Any(o => o[1] == mostHoursPlayer.Username))
            {
                options[random.Next(options.Count)] = new List<string> { mostHoursPlayer.AvatarUrl, mostHoursPlayer.Username };
                answerIndex = options.FindIndex(o => o[1] == mostHoursPlayer.Username);
            }

            var questionText = $"Who has the most hours on {game}?";

            return new Question(questionText,"", options, answerIndex,15);
        }

        // Selects a random game that at least 2 players have played, and asks which player has played it most recently
        private Question GenerateMostRecentlyPlayedQuestion(List<Player> players)
        {
            var gamesPlayedByMultiple = players
                .SelectMany(p => p.SteamItems)
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

            var mostRecentlyPlayedPlayer = relevantPlayers
                .OrderByDescending(p => p.SteamItems.First(i => i.Name == game).TimeLastPlayed)
                .First();

            var timelastplayed = mostRecentlyPlayedPlayer.SteamItems.First(i => i.Name == game).TimeLastPlayed;

            var options = relevantPlayers.Select(p => new List<string> { p.AvatarUrl, p.Username }).ToList();
            var answerIndex = options.FindIndex(o => o[1] == mostRecentlyPlayedPlayer.Username);

            options = options.OrderBy(x => random.Next()).ToList();

            if (!options.Any(o => o[1] == mostRecentlyPlayedPlayer.Username))
            {
                options[random.Next(options.Count)] = [mostRecentlyPlayedPlayer.AvatarUrl, mostRecentlyPlayedPlayer.Username];
                answerIndex = options.FindIndex(o => o[1] == mostRecentlyPlayedPlayer.Username);
            }

            var questionText = $"Who most recently played {game}? ({timelastplayed/60/24} days ago)";

            return new Question(questionText, "", options, answerIndex, 15);
        }

        // Selects a random player, and 4 of their games, and asks which game they have played the most
        private Question GenerateMostPlayedGameQuestion(List<Player> players)
        {
            var player = players[random.Next(players.Count)];
            var steamItems = player.SteamItems.OrderByDescending(i => i.HoursPlayed).Take(4).ToList();

            if (steamItems.Count < 4)
                return new Question(player.Username + " does not own 4 steam games. SHAME", "", [], -1, 5);

            var mostPlayedGame = steamItems.OrderByDescending(i => i.HoursPlayed).First();

            var options = steamItems.Select(i => new List<string> { i.ImageUrl, i.Name }).ToList();
            var answerIndex = options.FindIndex(o => o[1] == mostPlayedGame.Name);

            options = [.. options.OrderBy(x => random.Next())];

            if (!options.Any(o => o[1] == mostPlayedGame.Name))
            {
                options[random.Next(options.Count)] = [mostPlayedGame.ImageUrl, mostPlayedGame.Name];
                answerIndex = options.FindIndex(o => o[1] == mostPlayedGame.Name);
            }

            var questionText = $"What game has {player.Username} played the most? ({mostPlayedGame.HoursPlayed} hours)";

            return new Question(questionText, player.AvatarUrl, options, answerIndex, 15);
        }

        // Selects a random player, and 4 of their games, and asks which game they played most recently
        private Question GenerateMostRecentlyPlayedSingle(List<Player> players)
        {
            var player = players[random.Next(players.Count)];
            var steamItems = player.SteamItems.OrderByDescending(i => i.TimeLastPlayed).Take(4).ToList();

            if (steamItems.Count < 4)
                return new Question(player.Username + " does not own 4 steam games. SHAME", "", [], -1, 5);

            var mostRecentlyPlayedGame = steamItems.OrderByDescending(i => i.TimeLastPlayed).First();

            var options = steamItems.Select(i => new List<string> { i.ImageUrl, i.Name }).ToList();
            var answerIndex = options.FindIndex(o => o[1] == mostRecentlyPlayedGame.Name);

            options = [.. options.OrderBy(x => random.Next())];

            if (!options.Any(o => o[1] == mostRecentlyPlayedGame.Name))
            {
                options[random.Next(options.Count)] = [mostRecentlyPlayedGame.ImageUrl, mostRecentlyPlayedGame.Name];
                answerIndex = options.FindIndex(o => o[1] == mostRecentlyPlayedGame.Name);
            }

            var questionText = $"Which game has {player.Username} played most recently? ({mostRecentlyPlayedGame.TimeLastPlayed/60/24} days ago)";

            return new Question(questionText, player.AvatarUrl, options, answerIndex, 10);
        }
    }
}
