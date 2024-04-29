using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ApplicationCommandPropertiesConverter))]
public abstract partial class ApplicationCommandProperties
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
    public Permissions? DefaultGuildUserPermissions { get; set; }

    /// <summary>
    /// Indicates whether the command is available in DMs with the app.
    /// </summary>
    [Obsolete($"Replaced by '{nameof(Contexts)}'.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("dm_permission")]
    public bool? DMPermission { get; set; }

    /// <summary>
    /// Indicates whether the command is enabled by default when the app is added to a guild.
    /// </summary>
    [Obsolete($"Replaced by '{nameof(DefaultGuildUserPermissions)}'.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_permission")]
    public bool? DefaultPermission { get; set; }

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

    public class ApplicationCommandPropertiesConverter : JsonConverter<ApplicationCommandProperties>
    {
        public override ApplicationCommandProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, ApplicationCommandProperties value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case SlashCommandProperties slashCommandProperties:
                    JsonSerializer.Serialize(writer, slashCommandProperties, Serialization.Default.SlashCommandProperties);
                    break;
                case UserCommandProperties userCommandProperties:
                    JsonSerializer.Serialize(writer, userCommandProperties, Serialization.Default.UserCommandProperties);
                    break;
                case MessageCommandProperties messageCommandProperties:
                    JsonSerializer.Serialize(writer, messageCommandProperties, Serialization.Default.MessageCommandProperties);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid {nameof(ApplicationCommandProperties)} value.");
            }
        }
    }
}
