using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationCommandOptions
{
    internal ApplicationCommandOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("options")]
    public IEnumerable<ApplicationCommandOptionProperties>? Options { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_member_permissions")]
    public Permission? DefaultGuildUserPermissions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("dm_permission")]
    public bool? DMPermission { get; set; }

    [Obsolete("Replaced by 'DefaultGuildUserPermissions'.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_permission")]
    public bool? DefaultPermission { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nsfw")]
    public bool? Nsfw { get; set; }

    [JsonSerializable(typeof(ApplicationCommandOptions))]
    public partial class ApplicationCommandOptionsSerializerContext : JsonSerializerContext
    {
        public static ApplicationCommandOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
