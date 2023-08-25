using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class EnumTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var type = parameter.NonNullableType;
        if (Enum.TryParse(type, value, out var result) && Enum.IsDefined(type, result!))
            return Task.FromResult(result);
        else
            throw new FormatException($"Invalid {type.Name}.");
    }

    public override IChoicesProvider<TContext>? ChoicesProvider { get; } = new EnumChoicesProvider();

    private class EnumChoicesProvider : IChoicesProvider<TContext>
    {
        [UnconditionalSuppressMessage("Trimming", "IL2075:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "Literal fields on enums can never be trimmed")]
        public IEnumerable<ApplicationCommandOptionChoiceProperties>? GetChoices(SlashCommandParameter<TContext> parameter)
        {
            var type = parameter.NonNullableType;
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            int length = fields.Length;
            if (length > 25)
                throw new InvalidOperationException($"'{type}' has too many values, max choices count is 25.");

            for (var i = 0; i < length; i++)
            {
                var field = fields[i];
                var value = ((IConvertible)field.GetRawConstantValue()!).ToDouble(null);

                var attribute = field.GetCustomAttribute<SlashCommandChoiceAttribute>();
                if (attribute is not null)
                {
                    var translationsProviderType = attribute.TranslationsProviderType;
                    if (translationsProviderType is null)
                        yield return new(attribute.Name ?? field.Name, value);
                    else
                    {
                        if (!translationsProviderType.IsAssignableTo(typeof(ITranslationsProvider)))
                            throw new InvalidOperationException($"'{translationsProviderType}' is not assignable to '{nameof(ITranslationsProvider)}'.");

                        yield return new(attribute.Name ?? field.Name, value)
                        {
                            NameLocalizations = ((ITranslationsProvider)Activator.CreateInstance(translationsProviderType)!).Translations,
                        };
                    }
                }
                else
                    yield return new(field.Name, value);
            }
        }
    }
}
