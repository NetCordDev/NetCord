using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Gateway.Voice;
internal class VoicePayloadProperties<T>
{
    [JsonPropertyName("op")]
    public VoiceOpcode Opcode { get; set; }

    [JsonPropertyName("d")]
    public T D { get; set; }

    public VoicePayloadProperties(VoiceOpcode opcode, T d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize(JsonTypeInfo<VoicePayloadProperties<T>> jsonTypeInfo) => JsonSerializer.SerializeToUtf8Bytes(this, jsonTypeInfo);
}

internal static partial class VoicePayloadProperties
{
    [JsonSerializable(typeof(VoicePayloadProperties<ProtocolProperties>))]
    internal partial class VoicePayloadPropertiesOfProtocolPropertiesSerializerContext : JsonSerializerContext
    {
        public static VoicePayloadPropertiesOfProtocolPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(VoicePayloadProperties<int>))]
    internal partial class VoicePayloadPropertiesOfInt32SerializerContext : JsonSerializerContext
    {
        public static VoicePayloadPropertiesOfInt32SerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(VoicePayloadProperties<SpeakingProperties>))]
    internal partial class VoicePayloadPropertiesOfSpeakingPropertiesSerializerContext : JsonSerializerContext
    {
        public static VoicePayloadPropertiesOfSpeakingPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(VoicePayloadProperties<VoiceIdentifyProperties>))]
    internal partial class VoicePayloadPropertiesOfVoiceIdentifyPropertiesSerializerContext : JsonSerializerContext
    {
        public static VoicePayloadPropertiesOfVoiceIdentifyPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(VoicePayloadProperties<VoiceResumeProperties>))]
    internal partial class VoicePayloadPropertiesOfVoiceResumePropertiesSerializerContext : JsonSerializerContext
    {
        public static VoicePayloadPropertiesOfVoiceResumePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
