using System.Reflection;

namespace NetCord.Services.SlashCommands.TypeReaders;

public class EnumTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options)
    {
        var type = parameter.Type;
        if (Enum.TryParse(type, value, out var result) && Enum.IsDefined(type, result!))
            return Task.FromResult(result);
        else
            throw new FormatException($"Invalid {type}");
    }

    public override IEnumerable<ApplicationCommandOptionChoiceProperties>? GetChoices(SlashCommandParameter<TContext> parameter)
    {
        var array = Enum.GetValues(parameter.Type);
        if (array.Length > 25)
            throw new InvalidOperationException($"{parameter.Type.FullName} has too many values, max choices count is 25");
        foreach (Enum e in array)
        {
            var eString = e.ToString();
            var attribute = parameter.Type.GetField(eString)!.GetCustomAttribute<SlashCommandChoiceAttribute>();
            yield return new(attribute != null ? attribute.Name : eString, Convert.ToDouble(e));
        }
    }
}