using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class CurrentGuildUserOptions
{
    /// <summary>
    /// New nickname, empty to remove nickname.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }

    [JsonSerializable(typeof(CurrentGuildUserOptions))]
    public partial class CurrentGuildUserOptionsSerializerContext : JsonSerializerContext
    {
        public static CurrentGuildUserOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
