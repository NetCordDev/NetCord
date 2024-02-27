using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class StringMenuProperties(string customId, IEnumerable<StringMenuSelectOptionProperties> options) : MenuProperties(customId)
{
    public override ComponentType ComponentType => ComponentType.StringMenu;

    [JsonPropertyName("options")]
    public IEnumerable<StringMenuSelectOptionProperties> Options { get; set; } = options;
}
