using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class StringMenuProperties : MenuProperties
{
    [JsonPropertyName("options")]
    public IEnumerable<StringMenuSelectOptionProperties> Options { get; set; }

    public StringMenuProperties(string customId, IEnumerable<StringMenuSelectOptionProperties> options) : base(customId, ComponentType.StringMenu)
    {
        Options = options;
    }

    [JsonSerializable(typeof(StringMenuProperties))]
    public partial class StringMenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static StringMenuPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
