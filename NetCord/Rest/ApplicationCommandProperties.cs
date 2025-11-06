using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

/// <summary>
/// Application commands are native ways to interact with apps in the Discord client.
/// </summary>
[JsonConverter(typeof(JsonSerializableConverter<ApplicationCommandProperties>))]
[GenerateMethodsForProperties]
public abstract partial class ApplicationCommandProperties : IJsonSerializable<ApplicationCommandProperties>
{
    private protected ApplicationCommandProperties(ApplicationCommandType type, string name)
    {
        Type = type;
        Name = name;
    }

    /// <summary>
    /// Type of the command.
    /// </summary>
    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; }

    /// <summary>
    /// Name of the command (1-32 characters).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; set; }

    /// <summary>
    /// Default required permissions to use the command.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_member_permissions")]
    public Permissions? DefaultGuildPermissions { get; set; }

    /// <summary>
    /// Installation context(s) where the command is available.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("integration_types")]
    public IEnumerable<ApplicationIntegrationType>? IntegrationTypes { get; set; }

    /// <summary>
    /// Interaction context(s) where the command can be used.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("contexts")]
    public IEnumerable<InteractionContextType>? Contexts { get; set; }

    /// <summary>
    /// Indicates whether the command is age-restricted.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }

    void IJsonSerializable<ApplicationCommandProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }

    private protected abstract void WriteTo(Utf8JsonWriter writer);
}
