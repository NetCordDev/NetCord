using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationCommandResolvedData
{
    [JsonPropertyName("users")]
    public IReadOnlyDictionary<ulong, JsonUser>? Users { get; set; }

    [JsonPropertyName("members")]
    public IReadOnlyDictionary<ulong, JsonGuildUser>? GuildUsers { get; set; }

    [JsonPropertyName("roles")]
    public IReadOnlyDictionary<ulong, JsonGuildRole>? Roles { get; set; }

    [JsonPropertyName("channels")]
    public IReadOnlyDictionary<ulong, JsonChannel>? Channels { get; set; }

    [JsonPropertyName("messages")]
    public IReadOnlyDictionary<ulong, JsonMessage>? Messages { get; set; }

    [JsonPropertyName("attachments")]
    public IReadOnlyDictionary<ulong, JsonAttachment> Attachments { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommandResolvedData))]
    public partial class JsonApplicationCommandResolvedDataSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandResolvedDataSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
