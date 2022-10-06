using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildRoleOptions
{
    internal GuildRoleOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("permissions")]
    public Permission? Permissions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("color")]
    public Color? Color { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("hoist")]
    public bool? Hoist { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("unicode_emoji")]
    public string? UnicodeIcon { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mentionable")]
    public bool? Mentionable { get; set; }

    [JsonSerializable(typeof(GuildRoleOptions))]
    public partial class GuildRoleOptionsSerializerContext : JsonSerializerContext
    {
        public static GuildRoleOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
