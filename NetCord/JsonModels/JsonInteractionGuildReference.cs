using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonInteractionGuildReference : JsonEntity
{
    [JsonPropertyName("features")]
    public string[] Features { get; set; }

    [JsonPropertyName("locale")]
    public string Locale { get; set; }
}
