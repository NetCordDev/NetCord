using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMessageActivity
{
    [JsonPropertyName("type")]
    public MessageActivityType Type { get; set; }

    [JsonPropertyName("party_id")]
    public string? PartyId { get; set; }

    [JsonSerializable(typeof(JsonMessageActivity))]
    public partial class JsonMessageActivitySerializerContext : JsonSerializerContext
    {
        public static JsonMessageActivitySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
