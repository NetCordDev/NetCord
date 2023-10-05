using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonThreadCurrentUser
{
    [JsonPropertyName("join_timestamp")]
    public DateTimeOffset JoinTimestamp { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonSerializable(typeof(JsonThreadCurrentUser))]
    public partial class JsonThreadCurrentUserSerializerContext : JsonSerializerContext
    {
        public static JsonThreadCurrentUserSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
