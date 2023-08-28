using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Services.EnumTypeReaders;

internal class SlashCommandEnumTypeReader : IEnumTypeReader
{
    private readonly FieldInfo[] _fields;
    private readonly IEnumTypeReader _typeReader;

    internal SlashCommandEnumTypeReader(Type enumType) : this(enumType, EnumHelper.GetFields(enumType))
    {
    }

    internal SlashCommandEnumTypeReader(Type enumType, FieldInfo[] fields)
    {
        _typeReader = EnumValueTypeReader.Create(enumType, _fields = fields, CultureInfo.InvariantCulture);
    }

    public bool TryRead(ReadOnlyMemory<char> input, [MaybeNullWhen(false)] out object value)
    {
        return _typeReader.TryRead(input, out value);
    }

    public IEnumerable<ApplicationCommandOptionChoiceProperties> GetChoices()
    {
        return _fields.Select(CreateChoice);
    }

    private static ApplicationCommandOptionChoiceProperties CreateChoice(FieldInfo field)
    {
        var value = ((IConvertible)field.GetRawConstantValue()!).ToDouble(null);

        ApplicationCommandOptionChoiceProperties choice;
        var attribute = field.GetCustomAttribute<SlashCommandChoiceAttribute>();
        if (attribute is null)
            choice = new(field.Name, value);
        else
        {
            var translationsProviderType = attribute.NameTranslationsProviderType;
            if (translationsProviderType is null)
                choice = new(attribute.Name ?? field.Name, value);
            else
            {
                if (!translationsProviderType.IsAssignableTo(typeof(ITranslationsProvider)))
                    throw new InvalidOperationException($"'{translationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'.");

                choice = new(attribute.Name ?? field.Name, value)
                {
                    NameLocalizations = ((ITranslationsProvider)Activator.CreateInstance(translationsProviderType)!).Translations,
                };
            }
        }

        return choice;
    }
}
