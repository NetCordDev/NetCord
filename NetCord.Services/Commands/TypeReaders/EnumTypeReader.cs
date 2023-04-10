using System.Reflection;

namespace NetCord.Services.Commands.TypeReaders;

public class EnumTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    private readonly object _lock = new();
    private readonly Dictionary<CommandParameter<TContext>, bool> _parameters = new();
    private readonly Dictionary<Type, bool> _types = new();

    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration)
    {
        var span = input.Span;
        var type = parameter.NonNullableElementType;

        bool byValue;
        lock (_lock)
        {
            if (!_parameters.TryGetValue(parameter, out byValue))
                _parameters[parameter] = byValue = parameter.Attributes.ContainsKey(typeof(AllowByValueAttribute))
                                                   || (_types.TryGetValue(type, out var byValueType) ? byValueType : (_types[type] = type.IsDefined(typeof(AllowByValueAttribute))));
        }

        if (byValue) // by value or by name
        {
            if (Enum.TryParse(type, span, configuration.IgnoreCase, out var value) && Enum.IsDefined(type, value!))
                return Task.FromResult(value);
        }
        else // by name
        {
            if (((uint)span[0] - '0') > 9 && Enum.TryParse(type, span, configuration.IgnoreCase, out var value))
                return Task.FromResult(value);
        }

        throw new FormatException($"Invalid {type.Name}.");
    }
}
