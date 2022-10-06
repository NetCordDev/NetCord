using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildWidgetChannel : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonSerializable(typeof(JsonGuildWidgetChannel))]
    public partial class JsonGuildWidgetChannelSerializerContext : JsonSerializerContext
    {
        public static JsonGuildWidgetChannelSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
