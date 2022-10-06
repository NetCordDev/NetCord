using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEntity
{
    [JsonPropertyName("id")]
    public virtual Snowflake Id { get; set; }
}
