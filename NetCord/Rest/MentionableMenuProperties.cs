using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MentionableMenuProperties : MenuProperties
{
    public MentionableMenuProperties(string customId) : base(customId, ComponentType.MentionableMenu)
    {
    }

    [JsonSerializable(typeof(MentionableMenuProperties))]
    public partial class MentionableMenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static MentionableMenuPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
