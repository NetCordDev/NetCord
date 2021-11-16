using System.Text.Json.Serialization;

namespace NetCord.JsonModels
{
    internal record JsonEntity
    {
        [JsonPropertyName("id")]
        public virtual DiscordId Id { get; init; }
    }
}
