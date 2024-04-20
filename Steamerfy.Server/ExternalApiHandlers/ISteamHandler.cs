using Steamerfy.Server.Models;

namespace Steamerfy.Server.ExternalApiHandlers
{
    public interface ISteamHandler
    {
        public Task<Player?> GetPlayerInfo(string steamId);
        public Task<SteamItem[]> GetPlayerItems(Player player);
    }
}
