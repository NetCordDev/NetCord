using System.Text.Json.Serialization;

namespace NetCord
{
    public class InteractionMessageBuilder
    {
        public bool? Tts { get; set; }

        public string? Content { get; set; }

        public List<MessageEmbed>? Embeds { get; set; }

        public AllowedMentions? AllowedMentions { get; set; }

        public bool Ephemeral { get; set; }

        public List<Component>? Components { get; set; }

        public InteractionMessage Build()
        {
            return new(this);
        }
    }

    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public class InteractionMessage
    {
        [JsonPropertyName("tts")]
        public bool? Tts { get; }

        [JsonPropertyName("content")]
        public string? Content { get; }

        [JsonPropertyName("embeds")]
        public List<MessageEmbed>? Embeds { get; }

        [JsonPropertyName("allowed_mentions")]
        public AllowedMentions? AllowedMentions { get; }

        [JsonPropertyName("flags")]
        public MessageFlags? Flags { get; }

        [JsonPropertyName("components")]
        public List<Component>? Components { get; }

        internal InteractionMessage(InteractionMessageBuilder builder)
        {
            Tts = builder.Tts;
            Content = builder.Content;
            Embeds = builder.Embeds;
            AllowedMentions = builder.AllowedMentions;
            if (builder.Ephemeral)
                Flags = MessageFlags.Ephemeral;
            Components = builder.Components;
        }
    }
}
