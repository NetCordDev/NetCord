using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonConverters.SafeStringEnumConverter<ConnectionType>))]
public enum ConnectionType
{
    [JsonPropertyName("battlenet")]
    BattleNet,

    [JsonPropertyName("bungie")]
    Bungie,

    [JsonPropertyName("domain")]
    Domain,

    [JsonPropertyName("ebay")]
    Ebay,

    [JsonPropertyName("epicgames")]
    EpicGames,

    [JsonPropertyName("facebook")]
    Facebook,

    [JsonPropertyName("github")]
    GitHub,

    [JsonPropertyName("instagram")]
    Instagram,

    [JsonPropertyName("leagueoflegends")]
    LeagueOfLegends,

    [JsonPropertyName("paypal")]
    PayPal,

    [JsonPropertyName("playstation")]
    PlayStation,

    [JsonPropertyName("reddit")]
    Reddit,

    [JsonPropertyName("riotgames")]
    RiotGames,

    [JsonPropertyName("roblox")]
    Roblox,

    [JsonPropertyName("spotify")]
    Spotify,

    [JsonPropertyName("skype")]
    Skype,

    [JsonPropertyName("steam")]
    Steam,

    [JsonPropertyName("tiktok")]
    TikTok,

    [JsonPropertyName("twitch")]
    Twitch,

    [JsonPropertyName("twitter")]
    Twitter,

    [JsonPropertyName("xbox")]
    Xbox,

    [JsonPropertyName("youtube")]
    YouTube,
}
