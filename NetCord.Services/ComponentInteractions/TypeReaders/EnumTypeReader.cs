using NetCord.Services.EnumTypeReaders;

namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class EnumTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    private readonly EnumTypeReaderManager<IEnumTypeReader, Type, ComponentInteractionParameter<TContext>, ComponentInteractionServiceConfiguration<TContext>> _enumTypeReaderManager;

    public unsafe EnumTypeReader()
    {
        _enumTypeReaderManager = new(&GetKey, (type, parameter, configuration) => EnumValueTypeReader.Create(type, configuration.CultureInfo));

        static Type GetKey(ComponentInteractionParameter<TContext> parameter) => parameter.NonNullableElementType;
    }

    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (_enumTypeReaderManager.GetTypeReader(parameter, configuration).TryRead(input, out var value))
            return new(TypeReaderResult.Success(value));

        return new(TypeReaderResult.ParseFail(parameter.Name));
    }
}
