using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Services.EnumTypeReaders;

internal class SlashCommandEnumTypeReader<TContext>(Type enumType, FieldInfo[] fields) : IEnumTypeReader, IChoicesProvider<TContext> where TContext : IApplicationCommandContext
{
    private readonly IEnumTypeReader _typeReader = EnumValueTypeReader.Create(enumType, fields, CultureInfo.InvariantCulture);

    internal SlashCommandEnumTypeReader(Type enumType) : this(enumType, EnumHelper.GetFields(enumType))
    {
    }

    public bool TryRead(ReadOnlyMemory<char> input, [MaybeNullWhen(false)] out object value)
    {
        return _typeReader.TryRead(input, out value);
    }

    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(SlashCommandParameter<TContext> parameter)
    {
        var localizationsProvider = parameter.LocalizationsProvider;

        var count = fields.Length;
        var choices = new ApplicationCommandOptionChoiceProperties[count];
        for (var i = 0; i < count; i++)
            choices[i] = await CreateChoiceAsync(fields[i]).ConfigureAwait(false);

        return choices;

        async ValueTask<ApplicationCommandOptionChoiceProperties> CreateChoiceAsync(FieldInfo field)
        {
            var value = ((IConvertible)field.GetRawConstantValue()!).ToDouble(null);

            var attribute = field.GetCustomAttribute<SlashCommandChoiceAttribute>();

            var name = attribute is null ? field.Name : attribute.Name;

            ApplicationCommandOptionChoiceProperties result = new(name, value);

            if (localizationsProvider is not null)
            {
                var localizations = await localizationsProvider.GetLocalizationsAsync([new EnumLocalizationPathSegment(enumType), new EnumFieldLocalizationPathSegment(field), NameLocalizationPathSegment.Instance]).ConfigureAwait(false);
                result.NameLocalizations = localizations;
            }

            return result;
        }
    }
}
