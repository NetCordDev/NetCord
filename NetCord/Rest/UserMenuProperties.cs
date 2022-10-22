using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class UserMenuProperties : MenuProperties
{
    public UserMenuProperties(string customId) : base(customId, ComponentType.UserMenu)
    {
    }

    [JsonSerializable(typeof(UserMenuProperties))]
    public partial class UserMenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static UserMenuPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
