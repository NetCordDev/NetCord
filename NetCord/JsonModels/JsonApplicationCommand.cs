using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationCommand : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; set; } = ApplicationCommandType.ChatInput;

    [JsonPropertyName("application_id")]
    public Snowflake ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandOption[]? Options { get; set; }

    [JsonPropertyName("default_member_permissions")]
    public Permission? DefaultGuildUserPermissions { get; set; }

    [JsonPropertyName("dm_permission")]
    public bool? DMPermission { get; set; }

    [JsonPropertyName("default_permission")]
    public bool DefaultPermission { get; set; } = true;

    [JsonPropertyName("version")]
    public Snowflake Version { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommand))]
    public partial class JsonApplicationCommandSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonApplicationCommand[]))]
    public partial class JsonApplicationCommandArraySerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
