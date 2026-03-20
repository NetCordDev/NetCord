using System.Collections.Concurrent;
using System.Reflection;

using NetCord.Services.EnumTypeReaders;

namespace NetCord.Services.Commands.TypeReaders;

public class EnumTypeReader<TContext> : CommandTypeParser<TContext> where TContext : ICommandContext
{
    private readonly EnumTypeReaderManager<IEnumTypeReader, CommandParameter<TContext>, CommandParameter<TContext>, CommandServiceConfiguration<TContext>> _enumTypeReaderManager;
    private readonly ConcurrentDictionary<Type, bool> _byValueTypes = [];

    public unsafe EnumTypeReader()
    {
        _enumTypeReaderManager = new(&GetKey, CreateTypeReader);

        static CommandParameter<TContext> GetKey(CommandParameter<TContext> parameter) => parameter;
    }

    private IEnumTypeReader CreateTypeReader(CommandParameter<TContext> parameter, CommandParameter<TContext> _, CommandServiceConfiguration<TContext> configuration)
    {
        var type = parameter.NonNullableElementType;

        if (parameter.Attributes.ContainsKey(typeof(AllowByValueAttribute)))
            return new EnumNameOrValueTypeReader(type, configuration.IgnoreCase, configuration.CultureInfo);

        var byValueType = _byValueTypes.GetOrAdd(type, static type => type.IsDefined(typeof(AllowByValueAttribute)));

        return byValueType ? new EnumNameOrValueTypeReader(type, configuration.IgnoreCase, configuration.CultureInfo) : new EnumNameTypeReader(type, configuration.IgnoreCase);
    }

    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (_enumTypeReaderManager.GetTypeReader(parameter, configuration).TryRead(input, out var value))
            return new(CommandTypeParserResult.Success(value));

        return new(CommandTypeParserResult.ParseFail(parameter.Name));
    }
}
