using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildFromGuildTemplateProperties
{
    public GuildFromGuildTemplateProperties(string name)
    {
        Name = name;
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    [JsonSerializable(typeof(GuildFromGuildTemplateProperties))]
    public partial class GuildFromGuildTemplatePropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildFromGuildTemplatePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
