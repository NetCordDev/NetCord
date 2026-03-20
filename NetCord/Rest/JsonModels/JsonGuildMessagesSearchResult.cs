using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildMessagesSearchResult
{
    [JsonPropertyName("doing_deep_historical_index")]
    public bool? DoingDeepHistoricalIndex { get; set; }

    [JsonPropertyName("total_results")]
    public int? TotalResults { get; set; }

    [JsonPropertyName("messages")]
    public JsonMessage[][]? Messages { get; set; }

    [JsonPropertyName("threads")]
    public JsonChannel[]? Threads { get; set; }

    [JsonPropertyName("members")]
    public JsonThreadUser[]? ThreadUsers { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("documents_indexed")]
    public int? DocumentsIndexed { get; set; }

    [JsonPropertyName("retry_after")]
    public int? RetryAfter { get; set; }
}
