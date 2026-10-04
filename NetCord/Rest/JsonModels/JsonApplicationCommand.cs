using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonApplicationCommand : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandType? Type { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public Dictionary<string, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public Dictionary<string, string>? DescriptionLocalizations { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandOption[]? Options { get; set; }

    [JsonPropertyName("default_member_permissions")]
    public Permissions? DefaultGuildPermissions { get; set; }

    [JsonPropertyName("nsfw")]
    public bool? Nsfw { get; set; }

    [JsonPropertyName("integration_types")]
    public ApplicationIntegrationType[]? IntegrationTypes { get; set; }

    [JsonPropertyName("contexts")]
    public InteractionContextType[]? Contexts { get; set; }

    [JsonPropertyName("version")]
    public ulong Version { get; set; }

    [JsonPropertyName("handler")]
    public EntryPointCommandHandlerType? Handler { get; set; }
}
