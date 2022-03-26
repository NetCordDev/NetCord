using System.Reflection;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class EnumTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options)
    {
        var type = parameter.Type;
        if (Enum.TryParse(type, value, out var result) && Enum.IsDefined(type, result!))
            return Task.FromResult(result);
        else
            throw new FormatException($"Invalid {type}");
    }

    public override IChoicesProvider<TContext>? ChoicesProvider { get; } = new EnumChoicesProvider();

    private class EnumChoicesProvider : IChoicesProvider<TContext>
    {
        public IEnumerable<ApplicationCommandOptionChoiceProperties>? GetChoices(SlashCommandParameter<TContext> parameter)
        {
            var array = Enum.GetValues(parameter.Type);
            if (array.Length > 25)
                throw new InvalidOperationException($"{parameter.Type.FullName} has too many values, max choices count is 25");
            foreach (Enum e in array)
            {
                var eString = e.ToString();
                var attribute = parameter.Type.GetField(eString)!.GetCustomAttribute<SlashCommandChoiceAttribute>();
                if (attribute != null)
                {
                    if (attribute.TranslateProviderType != null)
                    {
                        if (!attribute.TranslateProviderType.IsAssignableTo(typeof(ITranslateProvider)))
                            throw new InvalidOperationException($"'{attribute.TranslateProviderType}' is not assignable to '{nameof(ITranslateProvider)}'");
                        yield return new(attribute.Name ?? eString, Convert.ToDouble(e))
                        {
                            NameLocalizations = ((ITranslateProvider)Activator.CreateInstance(attribute.TranslateProviderType)!).Translations
                        };
                    }
                    else
                        yield return new(attribute.Name ?? eString, Convert.ToDouble(e));
                }
                else
                    yield return new(eString, Convert.ToDouble(e));
            }
        }
    }
}