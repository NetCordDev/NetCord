using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildTemplateOptions
{
    internal GuildTemplateOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonSerializable(typeof(GuildTemplateOptions))]
    public partial class GuildTemplateOptionsSerializerContext : JsonSerializerContext
    {
        public static GuildTemplateOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
