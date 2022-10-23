using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildRolePositionProperties
{
    [JsonPropertyName("id")]
    public ulong Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    public GuildRolePositionProperties(ulong id)
    {
        Id = id;
    }

    [JsonSerializable(typeof(GuildRolePositionProperties))]
    public partial class GuildRolePositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildRolePositionPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(IEnumerable<GuildRolePositionProperties>))]
    public partial class IEnumerableOfGuildRolePositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfGuildRolePositionPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
