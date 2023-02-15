using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<ConnectionType>))]
public enum ConnectionType
{
    BattleNet,
    Ebay,
    EpicGames,
    Facebook,
    GitHub,
    Instagram,
    LeagueOfLegends,
    PayPal,
    PlayStation,
    Reddit,
    RiotGames,
    Spotify,
    Skype,
    Steam,
    TikTok,
    Twitch,
    Twitter,
    Xbox,
    YouTube,
}
