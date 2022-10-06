using System.Text.Json.Serialization;

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
    public Permission Permissions { get; set; }

    [JsonPropertyName("managed")]
    public bool Managed { get; set; }

    [JsonPropertyName("mentionable")]
    public bool Mentionable { get; set; }

    [JsonPropertyName("tags")]
    public JsonTags? Tags { get; set; }

    [JsonSerializable(typeof(JsonGuildRole))]
    public partial class JsonGuildRoleSerializerContext : JsonSerializerContext
    {
        public static JsonGuildRoleSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonGuildRole[]))]
    public partial class JsonGuildRoleArraySerializerContext : JsonSerializerContext
    {
        public static JsonGuildRoleArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

public partial class JsonTags
{
    [JsonPropertyName("bot_id")]
    public Snowflake? BotId { get; set; }

    [JsonPropertyName("integration_id")]
    public Snowflake? IntegrationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.NullConverter))]
    [JsonPropertyName("premium_subscriber")]
    public bool IsPremiumSubscriber { get; set; }

    [JsonSerializable(typeof(JsonTags))]
    public partial class JsonTagsSerializerContext : JsonSerializerContext
    {
        public static JsonTagsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
