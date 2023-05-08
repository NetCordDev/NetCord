using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

internal class MustContainAttribute : ParameterPreconditionAttribute<SlashCommandContext>
{
    private readonly string _value;

    public MustContainAttribute(string value)
    {
        _value = value;
    }

    public override ValueTask EnsureCanExecuteAsync(object? value, SlashCommandContext context, IServiceProvider? serviceProvider)
    {
        if (!((string)value!).Contains(_value))
            throw new($"The parameter must contain '{_value}'!");

        return default;
    }
}
