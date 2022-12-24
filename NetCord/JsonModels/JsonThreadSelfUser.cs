using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonThreadSelfUser
{
    [JsonPropertyName("join_timestamp")]
    public DateTimeOffset JoinTimestamp { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonSerializable(typeof(JsonThreadSelfUser))]
    public partial class JsonThreadSelfUserSerializerContext : JsonSerializerContext
    {
        public static JsonThreadSelfUserSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
