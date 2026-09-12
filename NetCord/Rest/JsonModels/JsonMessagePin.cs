using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

/// <summary>
/// Represents a pinned message in a channel.
/// </summary>
public class JsonMessagePin : JsonEntity
{
    /// <summary>
    /// The time the message was pinned.
    /// </summary>
    [JsonPropertyName("pinned_at")]
    public DateTimeOffset PinnedAt { get; set; }

    /// <summary>
    /// The pinned message.
    /// </summary>
    [JsonPropertyName("message")]
    public JsonMessage Message { get; set; }
}