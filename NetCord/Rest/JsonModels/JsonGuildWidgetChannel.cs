using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public partial class JsonGuildWidgetChannel : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonSerializable(typeof(JsonGuildWidgetChannel))]
    public partial class JsonGuildWidgetChannelSerializerContext : JsonSerializerContext
    {
        public static JsonGuildWidgetChannelSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
