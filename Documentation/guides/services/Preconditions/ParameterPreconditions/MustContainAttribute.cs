using NetCord.Services;

namespace MyBot;

// We use generics to make our attribute usable for text commands, application commands and interactions at the same time
public class MustContainAttribute<TContext>(string required) : ParameterPreconditionAttribute<TContext>
{
    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        // Return a fail result if does not contain
        if (!((string)value!).Contains(required, StringComparison.InvariantCultureIgnoreCase))
            return new(PreconditionResult.Fail($"The parameter must contain '{required}'."));

        return new(PreconditionResult.Success);
    }
}
