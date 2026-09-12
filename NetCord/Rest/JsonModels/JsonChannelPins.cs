using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

/// <summary>
/// Represents a list of pinned messages in a channel.
/// </summary>
public class JsonChannelPins : JsonEntity
{
    /// <summary>
    /// The list of pinned messages in the channel.
    /// </summary>
    [JsonPropertyName("items")]
    public JsonMessagePin[] Items { get; set; }

    /// <summary>
    /// Whether the list of pinned messages is partial.
    /// </summary>
    /// <value>
    /// true if there are more pinned messages to retrieve; otherwise, false.
    /// </value>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}