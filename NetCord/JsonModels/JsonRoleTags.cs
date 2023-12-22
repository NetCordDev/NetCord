using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonRoleTags
{
    [JsonPropertyName("bot_id")]
    public ulong? BotId { get; set; }

    [JsonPropertyName("integration_id")]
    public ulong? IntegrationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.NullConverter))]
    [JsonPropertyName("premium_subscriber")]
    public bool IsPremiumSubscriber { get; set; }

    [JsonPropertyName("subscription_listing_id")]
    public ulong? SubscriptionListingId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.NullConverter))]
    [JsonPropertyName("available_for_purchase")]
    public bool IsAvailableForPurchase { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.NullConverter))]
    [JsonPropertyName("guild_connections")]
    public bool GuildConnections { get; set; }
}
