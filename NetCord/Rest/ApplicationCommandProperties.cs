using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class SlashCommandProperties : ApplicationCommandProperties
{
    [JsonPropertyName("description")]
    public string Description { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("options")]
    public IEnumerable<ApplicationCommandOptionProperties>? Options { get; set; }

    public SlashCommandProperties(string name, string description) : base(name, ApplicationCommandType.ChatInput)
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
    public UserCommandProperties(string name) : base(name, ApplicationCommandType.User)
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
    public MessageCommandProperties(string name) : base(name, ApplicationCommandType.Message)
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
    private protected ApplicationCommandProperties(string name, ApplicationCommandType type)
    {
        Name = name;
        Type = type;
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

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
