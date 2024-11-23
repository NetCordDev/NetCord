using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

internal class MustContainAttribute(string value) : ParameterPreconditionAttribute<SlashCommandContext>
{
    private readonly string _value = value;

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, SlashCommandContext context, IServiceProvider? serviceProvider)
    {
        if (!((string)value!).Contains(_value))
            return new(PreconditionResult.Fail($"The parameter must contain '{_value}'!"));

        return new(PreconditionResult.Success);
    }
}
