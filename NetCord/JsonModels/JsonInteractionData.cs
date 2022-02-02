﻿using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonInteractionData
{
    [JsonPropertyName("id")]
    public DiscordId? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("type")]
    public ApplicationCommandType? Type { get; init; }

    [JsonPropertyName("resolved")]
    public JsonApplicationCommandResolvedData? ResolvedData { get; init; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandInteractionDataOption[]? Options { get; init; }

    [JsonPropertyName("custom_id")]
    public string? CustomId { get; init; }

    [JsonPropertyName("component_type")]
    public ComponentType? ComponentType { get; init; }

    [JsonPropertyName("values")]
    public string[]? SelectedValues { get; init; }

    [JsonPropertyName("target_id")]
    public DiscordId? MessageId { get; init; }
}