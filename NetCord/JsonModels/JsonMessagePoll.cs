using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonMessagePoll : JsonEntity
{
    [JsonPropertyName("question")]
    public JsonMessagePollMedia Question { get; set; }

    [JsonPropertyName("answers")]
    public JsonMessagePollAnswer[] Answers { get; set; }

    [JsonPropertyName("expiry")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }

    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType LayoutType { get; set; }

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

[JsonGuard]
public partial class JsonMessagePollAnswer
{
    [JsonPropertyName("answer_id")]
    public int AnswerId { get; set; }

    [JsonPropertyName("poll_media")]
    public JsonMessagePollMedia PollMedia { get; set; }
}

[JsonGuard]
public partial class JsonMessagePollResults : JsonEntity
{
    [JsonPropertyName("is_finalized")]
    public bool IsFinalized { get; set; }

    [JsonPropertyName("answer_counts")]
    public JsonMessagePollAnswerCount[] Answers { get; set; }
}

public class JsonMessagePollAnswerCount
{
    [JsonPropertyName("id")]
    public int AnswerId { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("me_voted")]
    public bool MeVoted { get; set; }
}
