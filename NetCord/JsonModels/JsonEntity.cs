using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEntity
{
    [JsonPropertyName("id")]
    public virtual ulong Id { get; set; }

    [JsonSerializable(typeof(JsonEntity[]))]
    public partial class JsonEntityArraySerializerContext : JsonSerializerContext
    {
        public static JsonEntityArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
