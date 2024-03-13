using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace NetCord.Services.ApplicationCommands;

public partial class JsonLocalizationsProvider(JsonLocalizationsProviderConfiguration? configuration = null) : ILocalizationsProvider
{
    private readonly record struct LocalizationInfo(string Locale, Localization Localization);

    private readonly JsonLocalizationsProviderConfiguration _configuration = configuration ?? new();

    private List<LocalizationInfo>? _localizationsInfo;

    private async ValueTask<List<LocalizationInfo>> LoadLocalizationsAsync()
    {
        var configuration = _configuration;

        var regex = CreateFileNameRegex(configuration.FileNameFormat, configuration.IgnoreCase);

        List<LocalizationInfo> result = [];

        var files = Directory.EnumerateFiles(configuration.DirectoryPath, "*", configuration.RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var match = regex.Match(name);
            if (!match.Success)
                continue;

            var locale = match.Groups[1].Value;

            Localization localization;

            using (AutoTranscodingStream stream = new(File.OpenRead(file), Encoding.UTF8))
                localization = (await JsonSerializer.DeserializeAsync(stream, LocalizationSerializerContext.Default.Localization).ConfigureAwait(false))!;

            result.Add(new(locale, localization));
        }

        return result;
    }

    private static Regex CreateFileNameRegex(string fileNameFormat, bool ignoreCase)
    {
        var escaped = Regex.Escape(fileNameFormat).AsSpan();

        int index = escaped.IndexOf("\\*");
        if (index == -1)
            throw new FormatException("The file name format must contain the '*' character indicating the locale.");

        StringBuilder stringBuilder = new(escaped.Length + 4);
        stringBuilder.Append('^');
        stringBuilder.Append(escaped[..index]);
        escaped = escaped[(index + 2)..];
        stringBuilder.Append("(.*)");

        while (true)
        {
            index = escaped.IndexOf("\\*");
            if (index == -1)
            {
                stringBuilder.Append(escaped);
                break;
            }

            stringBuilder.Append(escaped[..index]);
            escaped = escaped[(index + 2)..];
            stringBuilder.Append("\\1");
        }

        stringBuilder.Append('$');

        var pattern = stringBuilder.ToString();
        var options = ignoreCase ? (RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) : RegexOptions.CultureInvariant;

        return new(pattern, options);
    }

    public async ValueTask<IReadOnlyDictionary<string, string>?> GetLocalizationsAsync(IReadOnlyList<LocalizationPathSegment> path)
    {
        var localizationsInfo = _localizationsInfo ??= await LoadLocalizationsAsync().ConfigureAwait(false);

        int count = path.Count;
        int max = count - 1;

        Dictionary<string, string> result = new(localizationsInfo.Count);

        foreach (var localizationInfo in localizationsInfo)
        {
            var locale = localizationInfo.Locale;
            object? json = localizationInfo.Localization;
            LocalizationPathSegment segment;
            for (int i = 0; i < max; i++)
            {
                if (json is null)
                    goto NextLocale;

                segment = path[i];

                switch (segment)
                {
                    case ApplicationCommandLocalizationPathSegment applicationCommandSegment:
                        {
                            if (json is not ICommandsLocalization commandsLocalization)
                                throw new InvalidOperationException($"Expected '{nameof(ICommandsLocalization)}' but got '{json.GetType()}'");

                            var commands = commandsLocalization.Commands;
                            if (commands is null || !commands.TryGetValue(applicationCommandSegment.Name, out var command))
                                goto NextLocale;

                            json = command;
                            break;
                        }
                    case SlashCommandGroupLocalizationPathSegment commandGroupSegment:
                        {
                            if (json is not ISubCommandsLocalization commandGroupsLocalization)
                                throw new InvalidOperationException($"Expected '{nameof(ISubCommandsLocalization)}' but got '{json.GetType()}'");

                            var subCommands = commandGroupsLocalization.SubCommands;
                            if (subCommands is null || !subCommands.TryGetValue(commandGroupSegment.Name, out var subCommand))
                                goto NextLocale;

                            json = subCommand;
                            break;
                        }
                    case SubSlashCommandLocalizationPathSegment subCommandSegment:
                        {
                            if (json is not ISubCommandsLocalization subCommandsLocalization)
                                throw new InvalidOperationException($"Expected '{nameof(ISubCommandsLocalization)}' but got '{json.GetType()}'");

                            var subCommands = subCommandsLocalization.SubCommands;
                            if (subCommands is null || !subCommands.TryGetValue(subCommandSegment.Name, out var subCommand))
                                goto NextLocale;

                            json = subCommand;
                            break;
                        }
                    case SubSlashCommandGroupLocalizationPathSegment subCommandGroupSegment:
                        {
                            if (json is not ISubCommandsLocalization subCommandGroupsLocalization)
                                throw new InvalidOperationException($"Expected '{nameof(ISubCommandsLocalization)}' but got '{json.GetType()}'");

                            var subCommands = subCommandGroupsLocalization.SubCommands;
                            if (subCommands is null || !subCommands.TryGetValue(subCommandGroupSegment.Name, out var subCommand))
                                goto NextLocale;

                            json = subCommand;
                            break;
                        }
                    case SlashCommandParameterLocalizationPathSegment parameterSegment:
                        {
                            if (json is not IParametersLocalization parametersLocalization)
                                throw new InvalidOperationException($"Expected '{nameof(IParametersLocalization)}' but got '{json.GetType()}'");

                            var parameters = parametersLocalization.Parameters;
                            if (parameters is null || !parameters.TryGetValue(parameterSegment.Name, out var parameter))
                                goto NextLocale;

                            json = parameter;
                            break;
                        }
                    case EnumLocalizationPathSegment enumSegment:
                        {
                            if (json is not IEnumsLocalization enumsLocalization)
                                throw new InvalidOperationException($"Expected '{nameof(IEnumsLocalization)} but got '{json.GetType()}'");

                            var enums = enumsLocalization.Enums;
                            if (enums is null || !enums.TryGetValue(enumSegment.Type.FullName!, out var @enum))
                                goto NextLocale;

                            json = @enum;
                            break;
                        }
                    case EnumFieldLocalizationPathSegment enumFieldSegment:
                        {
                            if (json is not IEnumLocalization enumLocalization)
                                throw new InvalidOperationException($"Expected '{nameof(IEnumLocalization)}' but got '{json.GetType()}'");

                            var fields = enumLocalization.Fields;
                            if (fields is null || !fields.TryGetValue(enumFieldSegment.Field.Name, out var enumField))
                                goto NextLocale;

                            json = enumField;
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Cannot match the path segment of type '{segment.GetType()}'");
                }
            }

            if (json is null)
                continue;

            segment = path[max];
            switch (segment)
            {
                case NameLocalizationPathSegment nameSegment:
                    {
                        if (json is not INameLocalization nameLocalization)
                            throw new InvalidOperationException($"Expected '{nameof(INameLocalization)}' but got '{json.GetType()}'");

                        var name = nameLocalization.Name;
                        if (name is not null)
                            result[locale] = name;

                        break;
                    }
                case DescriptionLocalizationPathSegment descriptionSegment:
                    {
                        if (json is not IDescriptionLocalization descriptionLocalization)
                            throw new InvalidOperationException($"Expected '{nameof(IDescriptionLocalization)}' but got '{json.GetType()}'");

                        var description = descriptionLocalization.Description;
                        if (description is not null)
                            result[locale] = description;

                        break;
                    }
                default:
                    throw new InvalidOperationException($"Cannot match the path segment of type '{segment.GetType()}'");
            }

            NextLocale:;
        }

        return result;
    }

    internal interface ICommandsLocalization
    {
        public IReadOnlyDictionary<string, CommandLocalization>? Commands { get; }
    }

    internal interface INameLocalization
    {
        public string? Name { get; }
    }

    internal interface IDescriptionLocalization
    {
        public string? Description { get; }
    }

    internal interface ISubCommandsLocalization
    {
        public IReadOnlyDictionary<string, CommandLocalization>? SubCommands { get; }
    }

    internal interface IParametersLocalization
    {
        public IReadOnlyDictionary<string, ParameterLocalization>? Parameters { get; }
    }

    internal interface IEnumsLocalization
    {
        public IReadOnlyDictionary<string, EnumLocalization>? Enums { get; }
    }

    internal interface IEnumLocalization
    {
        public IReadOnlyDictionary<string, EnumFieldLocalization>? Fields { get; }
    }

    internal class Localization : ICommandsLocalization, IEnumsLocalization
    {
        [JsonPropertyName("commands")]
        public IReadOnlyDictionary<string, CommandLocalization>? Commands { get; set; }

        [JsonPropertyName("enums")]
        public IReadOnlyDictionary<string, EnumLocalization>? Enums { get; set; }
    }

    internal class CommandLocalization : INameLocalization, IDescriptionLocalization, ISubCommandsLocalization, IParametersLocalization
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("subcommands")]
        public IReadOnlyDictionary<string, CommandLocalization>? SubCommands { get; set; }

        [JsonPropertyName("parameters")]
        public IReadOnlyDictionary<string, ParameterLocalization>? Parameters { get; set; }
    }

    internal class SubCommandLocalization : INameLocalization, IDescriptionLocalization, ISubCommandsLocalization
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("subcommands")]
        public IReadOnlyDictionary<string, CommandLocalization>? SubCommands { get; set; }
    }

    internal class ParameterLocalization : INameLocalization, IDescriptionLocalization
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    [JsonConverter(typeof(EnumLocalizationConverter))]
    internal class EnumLocalization(IReadOnlyDictionary<string, EnumFieldLocalization>? fields) : IEnumLocalization
    {
        public IReadOnlyDictionary<string, EnumFieldLocalization>? Fields { get; set; } = fields;

        internal class EnumLocalizationConverter : JsonConverter<EnumLocalization>
        {
            public override EnumLocalization? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new(JsonSerializer.Deserialize(ref reader, LocalizationSerializerContext.Default.IReadOnlyDictionaryStringEnumFieldLocalization));
            }

            public override void Write(Utf8JsonWriter writer, EnumLocalization value, JsonSerializerOptions options)
            {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                JsonSerializer.Serialize(writer, value.Fields, LocalizationSerializerContext.Default.IReadOnlyDictionaryStringEnumFieldLocalization);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            }
        }
    }

    [JsonConverter(typeof(EnumFieldLocalizationConverter))]
    internal class EnumFieldLocalization(string name) : INameLocalization
    {
        public string? Name { get; set; } = name;

        internal class EnumFieldLocalizationConverter : JsonConverter<EnumFieldLocalization>
        {
            public override EnumFieldLocalization? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, EnumFieldLocalization value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Name);
            }
        }
    }

    [JsonSerializable(typeof(Localization))]
    [JsonSerializable(typeof(IReadOnlyDictionary<string, EnumFieldLocalization>))]
    internal partial class LocalizationSerializerContext : JsonSerializerContext
    {
    }
}

public class JsonLocalizationsProviderConfiguration
{
    public string DirectoryPath { get; init; } = "Localizations";

    public bool RecurseSubdirectories { get; init; }

    public string FileNameFormat { get; init; } = "*.json";

    public bool IgnoreCase { get; init; } = true;
}
