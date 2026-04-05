using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonInviteTargetUsersJobStatus
{
    [JsonPropertyName("status")]
    public InviteTargetUsersJobStatusCode Status { get; set; }

    [JsonPropertyName("total_users")]
    public int TotalUsers { get; set; }

    [JsonPropertyName("processed_users")]
    public int ProcessedUsers { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}
