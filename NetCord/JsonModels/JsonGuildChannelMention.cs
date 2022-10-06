using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildChannelMention : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("type")]
    public ChannelType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonSerializable(typeof(JsonGuildChannelMention))]
    public partial class JsonGuildChannelMentionSerializerContext : JsonSerializerContext
    {
        public static JsonGuildChannelMentionSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
