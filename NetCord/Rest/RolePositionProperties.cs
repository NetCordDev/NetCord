using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class RolePositionProperties
{
    [JsonPropertyName("id")]
    public ulong Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    public RolePositionProperties(ulong id)
    {
        Id = id;
    }

    [JsonSerializable(typeof(RolePositionProperties))]
    public partial class RolePositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static RolePositionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<RolePositionProperties>))]
    public partial class IEnumerableOfRolePositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfRolePositionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
