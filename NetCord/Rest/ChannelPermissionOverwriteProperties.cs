using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ChannelPermissionOverwriteProperties
{
    [JsonPropertyName("id")]
    public Snowflake Id { get; }

    [JsonPropertyName("type")]
    public PermissionOverwriteType Type { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allow")]
    public Permission? Allowed { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("deny")]
    public Permission? Denied { get; set; }

    public ChannelPermissionOverwriteProperties(Snowflake id, PermissionOverwriteType type)
    {
        Id = id;
        Type = type;
    }

    [JsonSerializable(typeof(ChannelPermissionOverwriteProperties))]
    public partial class ChannelPermissionOverwritePropertiesSerializerContext : JsonSerializerContext
    {
        public static ChannelPermissionOverwritePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
