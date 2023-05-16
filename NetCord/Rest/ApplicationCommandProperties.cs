using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class SlashCommandProperties : ApplicationCommandProperties
{
    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

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
    /// 
    /// </summary>
    /// <param name="name">Name of the command (1-32 characters).</param>
    /// <param name="description">Description of the command (1-100 characters).</param>
    public SlashCommandProperties(string name, string description) : base(ApplicationCommandType.ChatInput, name)
    {
        Description = description;
    }

    [JsonSerializable(typeof(SlashCommandProperties))]
    public partial class SlashCommandPropertiesSerializerContext : JsonSerializerContext
    {
        public static SlashCommandPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class UserCommandProperties : ApplicationCommandProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Name of the command (1-32 characters).</param>
    public UserCommandProperties(string name) : base(ApplicationCommandType.User, name)
    {
    }

    [JsonSerializable(typeof(UserCommandProperties))]
    public partial class UserCommandPropertiesSerializerContext : JsonSerializerContext
    {
        public static UserCommandPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class MessageCommandProperties : ApplicationCommandProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Name of the command (1-32 characters).</param>
    public MessageCommandProperties(string name) : base(ApplicationCommandType.Message, name)
    {
    }

    [JsonSerializable(typeof(MessageCommandProperties))]
    public partial class MessageCommandPropertiesSerializerContext : JsonSerializerContext
    {
        public static MessageCommandPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

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
    /// Translations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }

    internal class ApplicationCommandPropertiesConverter : JsonConverter<ApplicationCommandProperties>
    {
        public override ApplicationCommandProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, ApplicationCommandProperties value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case SlashCommandProperties slashCommandProperties:
                    JsonSerializer.Serialize(writer, slashCommandProperties, SlashCommandProperties.SlashCommandPropertiesSerializerContext.WithOptions.SlashCommandProperties);
                    break;
                case UserCommandProperties userCommandProperties:
                    JsonSerializer.Serialize(writer, userCommandProperties, UserCommandProperties.UserCommandPropertiesSerializerContext.WithOptions.UserCommandProperties);
                    break;
                case MessageCommandProperties messageCommandProperties:
                    JsonSerializer.Serialize(writer, messageCommandProperties, MessageCommandProperties.MessageCommandPropertiesSerializerContext.WithOptions.MessageCommandProperties);
                    break;
            }
        }
    }

    [JsonSerializable(typeof(ApplicationCommandProperties))]
    public partial class ApplicationCommandPropertiesSerializerContext : JsonSerializerContext
    {
        public static ApplicationCommandPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<ApplicationCommandProperties>))]
    public partial class IEnumerableOfApplicationCommandPropertiesSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfApplicationCommandPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
