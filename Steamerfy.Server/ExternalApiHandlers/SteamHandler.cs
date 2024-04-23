using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steamerfy.Server.Models;
using System.Linq;
using Steamerfy.Server.Models.PlayerDataClasses;

namespace Steamerfy.Server.ExternalApiHandlers
{
    public class SteamHandler : ISteamHandler
    {
        private readonly HttpClient _httpClient;
        private const string SteamApiKey = "YOUR_STEAM_API_KEY"; // Replace with your Steam API key

        public SteamHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Player?> GetPlayer(string steamId)
        {
            ProfileInfo profileInfo = await GetPlayerInfo(steamId);
            GameInfo gameInfo = await GetPlayerItems(steamId);
            return new Player(profileInfo, gameInfo, 0);
        }
        public async Task<ProfileInfo> GetPlayerInfo(string steamId)
        {
            // Construct the URL for the Steam Web API
            string url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={SteamApiKey}&steamids={steamId}";
            ProfileInfo profileinfo = new();
            // Send the HTTP request
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var steamResponse = JsonConvert.DeserializeObject<SteamPlayerSummariesResponse>(jsonResponse);
                if (steamResponse == null || steamResponse.Response == null || steamResponse.Response.Players == null || steamResponse.Response.Players.Length == 0)
                {
                    return profileinfo;
                }

                // Populate the Player object
                profileinfo.SteamId = steamResponse.Response.Players[0].SteamId;
                profileinfo.Username = steamResponse.Response.Players[0].Personaname;
                profileinfo.ProfileUrl = steamResponse.Response.Players[0].Profileurl;
                profileinfo.AvatarUrl = steamResponse.Response.Players[0].Avatarfull;
            }

            return profileinfo;
        }

        public async Task<GameInfo> GetPlayerItems(string steamId)
        {
            // Construct the URL for the Steam Web API to get the player's inventory
            string url = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={SteamApiKey}&steamid={steamId}&include_appinfo=1&include_played_free_games=1&format=json";
            GameInfo gameinfo = new();
            // Send the HTTP request
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var steamResponse = JsonConvert.DeserializeObject<SteamPlayerItemsResponse>(jsonResponse);
                if (steamResponse == null || steamResponse.Result == null || steamResponse.Result.Items == null || steamResponse.Result.Items.Length == 0)
                {
                    return gameinfo;
                }

                // Populate the SteamItem array
                SteamItem[] items = steamResponse.Result.Items.Select(item => new SteamItem(
                    name: item.Name ?? "Unknown",
                    imageUrl: $"http://media.steampowered.com/steamcommunity/public/images/items/730/{item.ImageUrl}.jpg",
                    hoursPlayed: item.PlaytimeForever / 60f,
                    priceCents: 0,
                    achievementCount: 0,
                    achievementTotal: 0,
                    achievementPercentage: 0f
                )).ToArray();
            }

            return gameinfo;
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

    public class SteamPlayerItemsResponse
    {
        [JsonProperty("result")]
        public PlayerItemsResult? Result { get; set; }
    }

    public class PlayerItemsResult
    {
        [JsonProperty("game_count")]
        public int GameCount { get; set; }
        [JsonProperty("games")]
        public SteamPlayerItem[]? Items { get; set; }
    }

    public class SteamPlayerItem
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("img_icon_url")]
        public string? ImageUrl { get; set; }

        [JsonProperty("playtime_forever")]
        public int PlaytimeForever { get; set; }
    }
}