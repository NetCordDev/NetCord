using NetCord.Services;

namespace MyBot;

// We use generics to make our attribute usable for text commands, application commands and interactions at the same time
public class MustContainAttribute<TContext> : ParameterPreconditionAttribute<TContext>
{
    private readonly string _value;

    public MustContainAttribute(string value)
    {
        _value = value;
    }

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        // Return a fail result if does not contain
        if (!((string)value!).Contains(_value, StringComparison.InvariantCultureIgnoreCase))
            return new(PreconditionResult.Fail($"The parameter must contain '{_value}'."));

        return new(PreconditionResult.Success);
    }
}
