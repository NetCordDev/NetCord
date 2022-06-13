using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonApplicationCommand : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; init; } = ApplicationCommandType.ChatInput;

    [JsonPropertyName("application_id")]
    public Snowflake ApplicationId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

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

    [JsonPropertyName("default_member_permissions")]
    public string? DefaultGuildUserPermissions { get; init; }

    [JsonPropertyName("dm_permission")]
    public bool? DMPermission { get; init; }

    [JsonPropertyName("default_permission")]
    public bool DefaultPermission { get; init; } = true;

    [JsonPropertyName("version")]
    public Snowflake Version { get; init; }
}