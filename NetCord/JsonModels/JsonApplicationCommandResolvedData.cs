using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationCommandResolvedData
{
    [JsonPropertyName("users")]
    public IReadOnlyDictionary<Snowflake, JsonUser>? Users { get; set; }

    [JsonPropertyName("members")]
    public IReadOnlyDictionary<Snowflake, JsonGuildUser>? GuildUsers { get; set; }

    [JsonPropertyName("roles")]
    public IReadOnlyDictionary<Snowflake, JsonGuildRole>? Roles { get; set; }

    [JsonPropertyName("channels")]
    public IReadOnlyDictionary<Snowflake, JsonChannel>? Channels { get; set; }

    [JsonPropertyName("messages")]
    public IReadOnlyDictionary<Snowflake, JsonMessage>? Messages { get; set; }

    [JsonPropertyName("attachments")]
    public IReadOnlyDictionary<Snowflake, JsonAttachment> Attachments { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommandResolvedData))]
    public partial class JsonApplicationCommandResolvedDataSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandResolvedDataSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
