using NetCord.Services;

namespace MyBot;

// We use generics to make our attribute usable for all types of commands and interactions at the same time
public class MustContainAttribute<TContext>(string required) : ParameterPreconditionAttribute<TContext>
{
    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        var text = (string)value!;

        // Return a fail result when the parameter value does not contain the required text
        if (!text.Contains(required, StringComparison.InvariantCultureIgnoreCase))
            return new(PreconditionResult.Fail($"The parameter must contain '{required}'."));

        return new(PreconditionResult.Success);
    }
}
