using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonRestGuildThreadResult
{
    [JsonPropertyName("threads")]
    public required JsonChannel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public required JsonThreadUser[] Users { get; set; }
}
