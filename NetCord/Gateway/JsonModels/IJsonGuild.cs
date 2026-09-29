using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            while (true)
            {
                if (!readerCopy.Read())
                    ThrowFailedToReadNextToken();

                var tokenType = readerCopy.TokenType;

                if (tokenType is JsonTokenType.PropertyName)
                {
                    if (readerCopy.ValueTextEquals("unavailable"u8))
                    {
                        if (!readerCopy.Read())
                            ThrowFailedToReadNextToken();

                        var unavailable = readerCopy.GetBoolean();

                        if (unavailable)
                            return JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUnavailableGuild);

                        break;
                    }
                    else
                        readerCopy.Skip();
                }
                else if (tokenType is JsonTokenType.EndObject)
                    break;
            }

            return JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonGuild);

            [DoesNotReturn]
            [StackTraceHidden]
            static void ThrowFailedToReadNextToken()
            {
                throw new JsonException("Failed to read the next JSON token.");
            }
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
