using NetCord.Services.EnumTypeReaders;

namespace NetCord.Services.Interactions.TypeReaders;

public class EnumTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    private readonly EnumTypeReaderManager<IEnumTypeReader, Type, InteractionParameter<TContext>, InteractionServiceConfiguration<TContext>> _enumTypeReaderManager;

    public unsafe EnumTypeReader()
    {
        _enumTypeReaderManager = new(&GetKey, (type, parameter, configuration) => EnumValueTypeReader.Create(type, configuration.CultureInfo));

        static Type GetKey(InteractionParameter<TContext> parameter) => parameter.NonNullableElementType;
    }

    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (_enumTypeReaderManager.GetTypeReader(parameter, configuration).TryRead(input, out var value))
            return new(value);

        throw new FormatException($"Invalid {parameter.NonNullableElementType.Name}.");
    }
}
