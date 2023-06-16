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

    [JsonSerializable(typeof(InteractionCallbackChoicesDataProperties))]
    public partial class InteractionCallbackChoicesDataPropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackChoicesDataPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
