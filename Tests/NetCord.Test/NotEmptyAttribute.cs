using NetCord.Services;
using NetCord.Services.Interactions;

namespace NetCord.Test;

internal class NotEmptyAttribute : ParameterPreconditionAttribute<StringMenuInteractionContext>
{
    public override ValueTask EnsureCanExecuteAsync(object? value, StringMenuInteractionContext context, IServiceProvider? serviceProvider)
    {
        if (((string)value!).Length == 0)
            throw new("The parameter cannot be empty!");

        return default;
    }
}
