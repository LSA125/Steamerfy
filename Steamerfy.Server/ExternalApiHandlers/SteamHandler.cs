using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steamerfy.Server.Models;

namespace Steamerfy.Server.ExternalApiHandlers
{
    public interface ISteamHandler
    {
        Task<Player?> GetPlayerInfo(string steamId);
    }

    public class SteamHandler : ISteamHandler
    {
        private readonly HttpClient _httpClient;
        private const string SteamApiKey = "YOUR_STEAM_API_KEY"; // Replace with your Steam API key

        public SteamHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Player?> GetPlayerInfo(string steamId)
        {
            // Construct the URL for the Steam Web API
            string url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={SteamApiKey}&steamids={steamId}";

            // Send the HTTP request
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var steamResponse = JsonConvert.DeserializeObject<SteamPlayerSummariesResponse>(jsonResponse);
                if (steamResponse == null || steamResponse.Response == null || steamResponse.Response.Players == null || steamResponse.Response.Players.Length == 0)
                {
                    return null;
                }

                // Populate the Player object
                Player player = new Player
                {
                    SteamId = steamId,
                    Username = steamResponse.Response.Players[0].Personaname,
                    ProfileUrl = steamResponse.Response.Players[0].Profileurl,
                    AvatarUrl = steamResponse.Response.Players[0].Avatarfull
                };

                return player;
            }

            return null;
        }

        public async Task<SteamItem[]> GetPlayerItems(Player player)
        {
            // You can implement the method to get the player's items from the Steam API here.
            // For now, returning an empty array.
            return new SteamItem[0];
        }
    }

    public class SteamPlayerSummariesResponse
    {
        [JsonProperty("response")]
        public PlayerSummariesResponse? Response { get; set; }
    }

    public class PlayerSummariesResponse
    {
        [JsonProperty("players")]
        public PlayerSummary[]? Players { get; set; }
    }

    public class PlayerSummary
    {
        [JsonProperty("steamid")]
        public string? SteamId { get; set; }

        [JsonProperty("personaname")]
        public string? Personaname { get; set; }

        [JsonProperty("profileurl")]
        public string? Profileurl { get; set; }

        [JsonProperty("avatarfull")]
        public string? Avatarfull { get; set; }
    }
}