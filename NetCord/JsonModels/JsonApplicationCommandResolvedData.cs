using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record class JsonApplicationCommandResolvedData
{
    [JsonPropertyName("users")]
    public IReadOnlyDictionary<ulong, JsonUser>? Users { get; init; }

    [JsonPropertyName("members")]
    public IReadOnlyDictionary<ulong, JsonGuildUser>? GuildUsers { get; init; }

    [JsonPropertyName("roles")]
    public IReadOnlyDictionary<ulong, JsonGuildRole>? Roles { get; init; }

    [JsonPropertyName("channels")]
    public IReadOnlyDictionary<ulong, JsonChannel>? Channels { get; init; }

    [JsonPropertyName("messages")]
    public IReadOnlyDictionary<ulong, JsonMessage>? Messages { get; init; }

    [JsonPropertyName("attachments")]
    public IReadOnlyDictionary<ulong, JsonAttachment> Attachments { get; init; }
}