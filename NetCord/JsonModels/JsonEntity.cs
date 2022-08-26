using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonEntity
{
    [JsonPropertyName("id")]
    public virtual Snowflake Id { get; init; }
}
