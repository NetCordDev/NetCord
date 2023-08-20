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

    public override ValueTask EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        // Throw exception if does not contain
        if (!((string)value!).Contains(_value, StringComparison.InvariantCultureIgnoreCase))
            throw new($"The parameter must contain '{_value}'.");

        return default;
    }
}
