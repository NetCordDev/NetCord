using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class BulkDeleteMessagesProperties
{
    public BulkDeleteMessagesProperties(Snowflake[] messageIds)
    {
        MessageIds = messageIds;
    }

    [JsonPropertyName("messages")]
    public Snowflake[] MessageIds { get; }

    [JsonSerializable(typeof(BulkDeleteMessagesProperties))]
    public partial class BulkDeleteMessagesPropertiesSerializerContext : JsonSerializerContext
    {
        public static BulkDeleteMessagesPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
