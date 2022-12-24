using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationInstallParams
{
    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("permissions")]
    public Permission Permissions { get; set; }

    [JsonSerializable(typeof(JsonApplicationInstallParams))]
    public partial class JsonApplicationInstallParamsSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationInstallParamsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
