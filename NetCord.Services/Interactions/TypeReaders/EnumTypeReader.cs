namespace NetCord.Services.Interactions.TypeReaders;

public class EnumTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var type = parameter.NonNullableElementType;
        if (Enum.TryParse(type, input.Span, configuration.IgnoreCase, out var value) && Enum.IsDefined(type, value!))
            return Task.FromResult(value);

        throw new FormatException($"Invalid {type.Name}.");
    }
}
