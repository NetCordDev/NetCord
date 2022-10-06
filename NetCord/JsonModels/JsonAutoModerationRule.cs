using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonAutoModerationRule : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("creator_id")]
    public Snowflake CreatorId { get; set; }

    [JsonPropertyName("event_type")]
    public AutoModerationRuleEventType EventType { get; set; }

    [JsonPropertyName("trigger_type")]
    public AutoModerationRuleTriggerType TriggerType { get; set; }

    [JsonPropertyName("trigger_metadata")]
    public JsonAutoModerationRuleTriggerMetadata TriggerMetadata { get; set; }

    [JsonPropertyName("actions")]
    public JsonAutoModerationAction[] Actions { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("exempt_roles")]
    public Snowflake[] ExemptRoles { get; set; }

    [JsonPropertyName("exempt_channels")]
    public Snowflake[] ExemptChannels { get; set; }

    [JsonSerializable(typeof(JsonAutoModerationRule))]
    public partial class JsonAutoModerationRuleSerializerContext : JsonSerializerContext
    {
        public static JsonAutoModerationRuleSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonAutoModerationRule[]))]
    public partial class JsonAutoModerationRuleArraySerializerContext : JsonSerializerContext
    {
        public static JsonAutoModerationRuleArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
