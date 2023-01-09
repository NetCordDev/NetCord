using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationCommandOptions
{
    internal ApplicationCommandOptions()
    {
    }

    /// <summary>
    /// Name of the command (1-32 characters).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Translations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Translations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    /// <summary>
    /// Parameters for the command (max 25).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("options")]
    public IEnumerable<ApplicationCommandOptionProperties>? Options { get; set; }

    /// <summary>
    /// Default required permissions to use the command.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_member_permissions")]
    public Permissions? DefaultGuildUserPermissions { get; set; }

    /// <summary>
    /// Indicates whether the command is available in DMs with the app.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("dm_permission")]
    public bool? DMPermission { get; set; }

    /// <summary>
    /// Indicates whether the command is enabled by default when the app is added to a guild.
    /// </summary>
    [Obsolete("Replaced by 'DefaultGuildUserPermissions'.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_permission")]
    public bool? DefaultPermission { get; set; }

    /// <summary>
    /// Indicates whether the command is age-restricted.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nsfw")]
    public bool? Nsfw { get; set; }

    [JsonSerializable(typeof(ApplicationCommandOptions))]
    public partial class ApplicationCommandOptionsSerializerContext : JsonSerializerContext
    {
        public static ApplicationCommandOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
