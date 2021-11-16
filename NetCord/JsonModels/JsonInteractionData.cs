using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonInteractionData : JsonEntity
{
    [JsonPropertyName("id")]
    public override DiscordId? Id { get; init; }

    [JsonPropertyName("name")]
    public string ApplicationCommandName { get; init; }

    [JsonPropertyName("type")]
    public ApplicationCommandType ApplicationCommandType { get; init; }

    [JsonPropertyName("resolved")]
    public JsonResolvedData ResolvedData { get; init; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandParameter[] Parameters { get; init; }

    [JsonPropertyName("custom_id")]
    public string CustomId { get; init; }

    [JsonPropertyName("component_type")]
    public MessageComponentType ComponentType { get; init; }

    [JsonPropertyName("values")]
    public string[] SelectedValues { get; init; }

    [JsonPropertyName("target_id")]
    public DiscordId? MessageId { get; init; }
}
