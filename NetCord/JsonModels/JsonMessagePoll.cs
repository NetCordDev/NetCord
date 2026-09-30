using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePoll : JsonEntity
{
    [JsonPropertyName("question")]
    public required JsonMessagePollMedia Question { get; set; }

    [JsonPropertyName("answers")]
    public required JsonMessagePollAnswer[] Answers { get; set; }

    [JsonPropertyName("expiry")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("allow_multiselect")]
    public required bool AllowMultiselect { get; set; }

    [JsonPropertyName("layout_type")]
    public required MessagePollLayoutType LayoutType { get; set; }

    [JsonPropertyName("results")]
    public JsonMessagePollResults? Results { get; set; }
}

public class JsonMessagePollMedia : JsonEntity
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }
}

public class JsonMessagePollAnswer
{
    [JsonPropertyName("answer_id")]
    public required int AnswerId { get; set; }

    [JsonPropertyName("poll_media")]
    public required JsonMessagePollMedia PollMedia { get; set; }
}

public class JsonMessagePollResults : JsonEntity
{
    [JsonPropertyName("is_finalized")]
    public required bool IsFinalized { get; set; }

    [JsonPropertyName("answer_counts")]
    public required JsonMessagePollAnswerCount[] Answers { get; set; }
}

public class JsonMessagePollAnswerCount
{
    [JsonPropertyName("id")]
    public required int AnswerId { get; set; }

    [JsonPropertyName("count")]
    public required int Count { get; set; }

    [JsonPropertyName("me_voted")]
    public required bool MeVoted { get; set; }
}
