using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

[JsonConverter(typeof(Converter))]
public interface IJsonInteractionData
{
    public class Converter : JsonConverter<IJsonInteractionData>
    {
        public override IJsonInteractionData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

        }

        public override void Write(Utf8JsonWriter writer, IJsonInteractionData value, JsonSerializerOptions options) => throw new NotSupportedException();
    }
}

public class JsonApplicationCommandData : IJsonInteractionData
{
    [JsonPropertyName("id")]
    public required ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required ApplicationCommandType Type { get; set; }

    [JsonPropertyName("resolved")]
    public JsonInteractionResolvedData? Resolved { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandInteractionDataOption[]? Options { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public ulong? TargetId { get; set; }
}

public interface IJsonComponentData : IJsonInteractionData
{
    public string CustomId { get; set; }
}

public class JsonMessageComponentData : IJsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("component_type")]
    public required ComponentType ComponentType { get; set; }

    [JsonPropertyName("values")]
    public string[]? Values { get; set; }

    [JsonPropertyName("resolved")]
    public JsonInteractionResolvedData? Resolved { get; set; }
}

public class JsonModalData : IJsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("components")]
    public required JsonComponent[] Components { get; set; }

    [JsonPropertyName("resolved")]
    public JsonInteractionResolvedData? Resolved { get; set; }
}

// public class JsonInteractionData
// {
//     [JsonPropertyName("id")]
//     public ulong? Id { get; set; }
//
//     [JsonPropertyName("name")]
//     public string? Name { get; set; }
//
//     [JsonPropertyName("type")]
//     public ApplicationCommandType? Type { get; set; }
//
//     [JsonPropertyName("resolved")]
//     public JsonInteractionResolvedData? ResolvedData { get; set; }
//
//     [JsonPropertyName("options")]
//     public JsonApplicationCommandInteractionDataOption[]? Options { get; set; }
//
//     [JsonPropertyName("guild_id")]
//     public ulong? GuildId { get; set; }
//
//     [JsonPropertyName("custom_id")]
//     public string? CustomId { get; set; }
//
//     [JsonPropertyName("component_type")]
//     public ComponentType? ComponentType { get; set; }
//
//     [JsonPropertyName("values")]
//     public string[]? SelectedValues { get; set; }
//
//     [JsonPropertyName("target_id")]
//     public ulong? TargetId { get; set; }
//
//     [JsonPropertyName("components")]
//     public JsonComponent[]? Components { get; set; }
// }
