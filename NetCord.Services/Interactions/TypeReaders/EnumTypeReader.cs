namespace NetCord.Services.Interactions.TypeReaders;

public class EnumTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options)
    {
        var type = parameter.Type;
        if (Enum.TryParse(type, input.Span, options.IgnoreCase, out var value) && Enum.IsDefined(type, value!))
            return Task.FromResult(value);

        throw new FormatException($"Invalid {type.Name}.");
    }
}
