using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;
using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Gateway.JsonModels;

[JsonConverter(typeof(JsonGuildConverter))]
public interface IJsonGuild
{
    public ulong Id { get; set; }

    public bool? IsUnavailable { get; set; }

    public class JsonGuildConverter : JsonConverter<IJsonGuild>
    {
        public override IJsonGuild? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;

            return JsonConverterHelper.TrySkipToProperty(ref readerCopy, "unavailable"u8) && readerCopy.GetBoolean()
                ? JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUnavailableGuild)
                : JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonGuild);
        }

        public override void Write(Utf8JsonWriter writer, IJsonGuild value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}

public class JsonUnavailableGuild : JsonEntity, IJsonGuild
{
    [JsonPropertyName("unavailable")]
    public bool? IsUnavailable { get; set; }
}

public class JsonGuild : JsonRestGuild, IJsonGuild
{
    [JsonPropertyName("joined_at")]
    public required DateTimeOffset JoinedAt { get; set; }

    [JsonPropertyName("large")]
    public required bool IsLarge { get; set; }

    [JsonPropertyName("unavailable")]
    public bool? IsUnavailable { get; set; }

    [JsonPropertyName("member_count")]
    public required int UserCount { get; set; }

    [JsonPropertyName("voice_states")]
    public required JsonVoiceState[] VoiceStates { get; set; }

    [JsonPropertyName("members")]
    public required JsonGuildUser[] Users { get; set; }

    [JsonPropertyName("channels")]
    public required JsonChannel[] Channels { get; set; }

    [JsonPropertyName("threads")]
    public required JsonChannel[] ActiveThreads { get; set; }

    [JsonPropertyName("presences")]
    public required JsonPresence[] Presences { get; set; }

    [JsonPropertyName("stage_instances")]
    public required JsonStageInstance[] StageInstances { get; set; }

    [JsonPropertyName("guild_scheduled_events")]
    public required JsonGuildScheduledEvent[] ScheduledEvents { get; set; }
}
