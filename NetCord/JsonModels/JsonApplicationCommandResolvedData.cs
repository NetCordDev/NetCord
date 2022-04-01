using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record class JsonApplicationCommandResolvedData
{
    [JsonPropertyName("users")]
    public IReadOnlyDictionary<Snowflake, JsonUser>? Users { get; init; }

    [JsonPropertyName("members")]
    public IReadOnlyDictionary<Snowflake, JsonGuildUser>? GuildUsers { get; init; }

    [JsonPropertyName("roles")]
    public IReadOnlyDictionary<Snowflake, JsonGuildRole>? Roles { get; init; }

    [JsonPropertyName("channels")]
    public IReadOnlyDictionary<Snowflake, JsonChannel>? Channels { get; init; }

    [JsonPropertyName("messages")]
    public IReadOnlyDictionary<Snowflake, JsonMessage>? Messages { get; init; }

    [JsonPropertyName("attachments")]
    public IReadOnlyDictionary<Snowflake, JsonAttachment> Attachments { get; init; }
}