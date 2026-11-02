using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

internal class NotEmptyAttribute : ParameterPreconditionAttribute<StringSelectInteractionContext>
{
    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, StringSelectInteractionContext context, IServiceProvider? serviceProvider)
    {
        if (((string)value!).Length == 0)
            return new(PreconditionResult.Fail("Value cannot be empty."));

        return new(PreconditionResult.Success);
    }
}
