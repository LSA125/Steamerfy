using Steamerfy.Server.Models;
using Steamerfy.Server.Models.PlayerDataClasses;

namespace Steamerfy.Server.ExternalApiHandlers
{
    public interface ISteamHandler
    {
        public Task<Player?> GetPlayer(string steamId);
    }
}
