using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Gateway;

internal class GatewayPayloadProperties<T>
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; }

    [JsonPropertyName("d")]
    public T D { get; }

    public GatewayPayloadProperties(GatewayOpcode opcode, T d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize(JsonTypeInfo<GatewayPayloadProperties<T>> jsonTypeInfo) => JsonSerializer.SerializeToUtf8Bytes(this, jsonTypeInfo);
}

internal static partial class GatewayPayloadProperties
{
    [JsonSerializable(typeof(GatewayPayloadProperties<GatewayIdentifyProperties>))]
    internal partial class GatewayPayloadPropertiesOfGatewayIdentifyPropertiesSerializerContext : JsonSerializerContext
    {
        public static GatewayPayloadPropertiesOfGatewayIdentifyPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(GatewayPayloadProperties<GatewayResumeProperties>))]
    internal partial class GatewayPayloadPropertiesOfGatewayResumePropertiesSerializerContext : JsonSerializerContext
    {
        public static GatewayPayloadPropertiesOfGatewayResumePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(GatewayPayloadProperties<int>))]
    internal partial class GatewayPayloadPropertiesOfInt32SerializerContext : JsonSerializerContext
    {
        public static GatewayPayloadPropertiesOfInt32SerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(GatewayPayloadProperties<VoiceStateProperties>))]
    internal partial class GatewayPayloadPropertiesOfVoiceStatePropertiesSerializerContext : JsonSerializerContext
    {
        public static GatewayPayloadPropertiesOfVoiceStatePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(GatewayPayloadProperties<PresenceProperties>))]
    internal partial class GatewayPayloadPropertiesOfPresencePropertiesSerializerContext : JsonSerializerContext
    {
        public static GatewayPayloadPropertiesOfPresencePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(GatewayPayloadProperties<GuildUsersRequestProperties>))]
    internal partial class GatewayPayloadPropertiesOfGuildUsersRequestPropertiesSerializerContext : JsonSerializerContext
    {
        public static GatewayPayloadPropertiesOfGuildUsersRequestPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
