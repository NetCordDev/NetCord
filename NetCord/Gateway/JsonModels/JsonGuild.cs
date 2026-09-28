using System.Text.Json.Serialization;

using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonGuild : JsonRestGuild
{
    [JsonPropertyName("joined_at")]
    public DateTimeOffset JoinedAt { get; set; }

    [JsonPropertyName("large")]
    public bool IsLarge { get; set; }

    [JsonPropertyName("unavailable")]
    public bool? IsUnavailable { get; set; }

    [JsonPropertyName("member_count")]
    public int UserCount { get; set; }

    [JsonPropertyName("voice_states")]
    public JsonVoiceState[] VoiceStates { get; set; }

    [JsonPropertyName("members")]
    public JsonGuildUser[] Users { get; set; }

    [JsonPropertyName("channels")]
    public JsonChannel[] Channels { get; set; }

    [JsonPropertyName("threads")]
    public JsonChannel[] ActiveThreads { get; set; }

    [JsonPropertyName("presences")]
    public JsonPresence[] Presences { get; set; }

    [JsonPropertyName("stage_instances")]
    public JsonStageInstance[] StageInstances { get; set; }

    [JsonPropertyName("guild_scheduled_events")]
    public JsonGuildScheduledEvent[] ScheduledEvents { get; set; }
}
