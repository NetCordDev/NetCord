using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonApplicationCommand : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; init; } = ApplicationCommandType.ChatInput;

    [JsonPropertyName("application_id")]
    public DiscordId ApplicationId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; init; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandOption[]? Options { get; init; }

    [JsonPropertyName("default_permission")]
    public bool DefaultPermission { get; init; } = true;

    [JsonPropertyName("version")]
    public DiscordId Version { get; init; }
}