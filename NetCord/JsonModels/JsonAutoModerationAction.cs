using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public record JsonAutoModerationAction
{
    [JsonPropertyName("type")]
    public AutoModerationActionType Type { get; init; }

    [JsonPropertyName("metadata")]
    public JsonAutoModerationActionMetadata? Metadata { get; init; }
}