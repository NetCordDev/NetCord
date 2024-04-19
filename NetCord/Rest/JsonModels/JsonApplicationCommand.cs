using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonApplicationCommand : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; set; } = ApplicationCommandType.ChatInput;

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandOption[]? Options { get; set; }

    [JsonPropertyName("default_member_permissions")]
    public Permissions? DefaultGuildUserPermissions { get; set; }

    [Obsolete($"Replaced by '{nameof(Contexts)}'.")]
    [JsonPropertyName("dm_permission")]
    public bool? DMPermission { get; set; }

    [Obsolete($"Replaced by '{nameof(DefaultGuildUserPermissions)}'.")]
    [JsonPropertyName("default_permission")]
    public bool DefaultPermission { get; set; } = true;

    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }

    [JsonPropertyName("integration_types")]
    public ApplicationIntegrationType[]? IntegrationTypes { get; set; }

    [JsonPropertyName("contexts")]
    public InteractionContextType[]? Contexts { get; set; }

    [JsonPropertyName("version")]
    public ulong Version { get; set; }
}
