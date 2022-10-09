﻿using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord;

public partial class InteractionCallbackChoicesDataProperties
{
    [JsonPropertyName("choices")]
    public IEnumerable<ApplicationCommandOptionChoiceProperties>? Choices { get; }

    public InteractionCallbackChoicesDataProperties(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
    {
        Choices = choices;
    }

    [JsonSerializable(typeof(InteractionCallbackChoicesDataProperties))]
    public partial class InteractionCallbackChoicesDataPropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackChoicesDataPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
