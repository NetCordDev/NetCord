using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class RoleMenuProperties : MenuProperties
{
    public RoleMenuProperties(string customId) : base(customId, ComponentType.RoleMenu)
    {
    }

    [JsonSerializable(typeof(RoleMenuProperties))]
    public partial class RoleMenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static RoleMenuPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
