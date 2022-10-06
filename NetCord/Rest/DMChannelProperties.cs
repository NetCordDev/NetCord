using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class DMChannelProperties
{
    public DMChannelProperties(Snowflake userId)
    {
        UserId = userId;
    }

    [JsonPropertyName("recipient_id")]
    public Snowflake UserId { get; }

    [JsonSerializable(typeof(DMChannelProperties))]
    public partial class DMChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static DMChannelPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
