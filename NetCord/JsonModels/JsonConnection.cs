using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public partial class JsonConnection : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public ConnectionType Type { get; set; }

    [JsonPropertyName("revoked")]
    public bool? Revoked { get; set; }

    [JsonPropertyName("integrations")]
    public JsonIntegration[]? Integrations { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("friend_sync")]
    public bool FriendSync { get; set; }

    [JsonPropertyName("show_activity")]
    public bool ShowActivity { get; set; }

    [JsonPropertyName("visibility")]
    public ConnectionVisibility Visibility { get; set; }

    [JsonSerializable(typeof(JsonConnection))]
    public partial class JsonConnectionSerializerContext : JsonSerializerContext
    {
        public static JsonConnectionSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonConnection[]))]
    public partial class JsonConnectionArraySerializerContext : JsonSerializerContext
    {
        public static JsonConnectionArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
