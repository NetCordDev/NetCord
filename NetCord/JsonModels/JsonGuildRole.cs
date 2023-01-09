﻿using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildRole : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("color")]
    public Color Color { get; set; }

    [JsonPropertyName("hoist")]
    public bool Hoist { get; set; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("unicode_emoji")]
    public string? UnicodeEmoji { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("permissions")]
    public Permissions Permissions { get; set; }

    [JsonPropertyName("managed")]
    public bool Managed { get; set; }

    [JsonPropertyName("mentionable")]
    public bool Mentionable { get; set; }

    [JsonPropertyName("tags")]
    public JsonGuildRoleTags? Tags { get; set; }

    [JsonSerializable(typeof(JsonGuildRole))]
    public partial class JsonGuildRoleSerializerContext : JsonSerializerContext
    {
        public static JsonGuildRoleSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonGuildRole[]))]
    public partial class JsonGuildRoleArraySerializerContext : JsonSerializerContext
    {
        public static JsonGuildRoleArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class JsonGuildRoleTags
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

    [JsonSerializable(typeof(JsonGuildRoleTags))]
    public partial class JsonTagsSerializerContext : JsonSerializerContext
    {
        public static JsonTagsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
