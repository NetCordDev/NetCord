using System.Text.Json.Serialization;

namespace NetCord
{
    public class MenuProperties : ComponentProperties
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; }

        [JsonPropertyName("options")]
        public List<SelectOption>? Options { get; init; }

        [JsonPropertyName("placeholder")]
        public string? Placeholder { get; init; }

        [JsonPropertyName("min_values")]
        public int? MinValues { get; init; }

        [JsonPropertyName("max_values")]
        public int? MaxValues { get; init; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; init; }

        public MenuProperties(string customId) : base(ComponentType.Menu)
        {
            CustomId = customId;
        }

        public class SelectOption
        {
            [JsonPropertyName("label")]
            public string Label { get; }

            [JsonPropertyName("value")]
            public string Value { get; }

            [JsonPropertyName("description")]
            public string? Description { get; init; }

            [JsonPropertyName("emoji")]
            [JsonConverter(typeof(JsonConverters.ComponentEmojiConverter))]
            public DiscordId? EmojiId { get; init; }

            [JsonPropertyName("default")]
            public bool? IsDefault { get; init; }

            public SelectOption(string label, string value)
            {
                Label = label;
                Value = value;
            }
        }
    }
}
