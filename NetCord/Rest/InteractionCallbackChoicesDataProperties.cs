using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class InteractionCallbackChoicesDataProperties
{
    [JsonPropertyName("choices")]
    public IEnumerable<ApplicationCommandOptionChoiceProperties>? Choices { get; set; }

    public InteractionCallbackChoicesDataProperties(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
    {
        Choices = choices;
    }
}
