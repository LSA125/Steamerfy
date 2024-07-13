using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steamerfy.Server.Models;
using System.Linq;
using Steamerfy.Server.Models.PlayerDataClasses;
using Steamerfy.Server.Services;

namespace Steamerfy.Server.ExternalApiHandlers
{
    public class SteamHandler : ISteamHandler
    {
        private readonly HttpClient _httpClient;
        private readonly string? SteamApiKey = Environment.GetEnvironmentVariable("STEAM_API_KEY");
        public SteamHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
            if (SteamApiKey == null)
            {
                throw new Exception("STEAM_API_KEY environment variable not set");
            }
            
        }
        public async Task<Player?> GetPlayer(string steamId)
        {
            ProfileInfo? profileInfo = await GetPlayerInfo(steamId);
            if (profileInfo == null)
            {
                return null;
            }
            List<SteamItem>? gameInfo = await GetPlayerItems(steamId);
            if (gameInfo == null)
            {
                return null;
            }
            return new Player(profileInfo, gameInfo, 0);
        }
        public async Task<ProfileInfo?> GetPlayerInfo(string steamId)
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
                var Personaname = steamResponse.Response.Players[0].Personaname;
                var Profileurl = steamResponse.Response.Players[0].Profileurl;
                var Avatarfull = steamResponse.Response.Players[0].Avatarfull;
                var SteamId = steamResponse.Response.Players[0].SteamId;
                // Populate the Player object
                if (Personaname == null || Profileurl == null || Avatarfull == null || SteamId == null)
                {
                    return null;
                }
                return new ProfileInfo( Username: Personaname,
                                        ProfileUrl: Profileurl,
                                        AvatarUrl: Avatarfull,
                                        SteamId: SteamId);
            }
            return null;
        }

        public async Task<List<SteamItem>?> GetPlayerItems(string steamId)
        {
            // Construct the URL for the Steam Web API to get the player's inventory
            string url = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={SteamApiKey}&steamid={steamId}&include_appinfo=1&include_played_free_games=1&format=json";
            // Send the HTTP request
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var steamResponse = JsonConvert.DeserializeObject<SteamPlayerItemsResponse>(jsonResponse);
                if (steamResponse == null || steamResponse.Response == null || steamResponse.Response.Items == null || steamResponse.Response.Items.Length == 0)
                {
                    return null;
                }

                // Populate the SteamItem array
                List<SteamItem> items = steamResponse.Response.Items.Select(item => new SteamItem(
                    name: item.Name ?? "Unknown",
                    imageUrl: $"http://media.steampowered.com/steamcommunity/public/images/apps/{item.AppId}/{item.ImageUrl}.jpg",
                    hoursPlayed: item.PlaytimeForever / 60,
                    TimeLastPlayed: UnixTimeToLastTimeInDays((uint)item.RtimeLastPlayed)
                )).ToList();
                return items;
            }
            return null;
        }

        private uint UnixTimeToLastTimeInDays(uint unixTime)
        {
            TimeSpan diff = DateTime.Now - DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
            return (uint)Math.Round(diff.TotalDays);
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
        [JsonProperty("response")]
        public PlayerItemsResponseData? Response { get; set; }
    }

    public class PlayerItemsResponseData
    {
        [JsonProperty("game_count")]
        public int GameCount { get; set; }

        [JsonProperty("games")]
        public SteamPlayerItem[]? Items { get; set; }
    }

    public class SteamPlayerItem
    {
        [JsonProperty("appid")]
        public int AppId { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("img_icon_url")]
        public string? ImageUrl { get; set; }

        [JsonProperty("playtime_forever")]
        public int PlaytimeForever { get; set; }

        [JsonProperty("rtime_last_played")]
        public int RtimeLastPlayed { get; set; }
    }
}