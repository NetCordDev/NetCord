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
}
