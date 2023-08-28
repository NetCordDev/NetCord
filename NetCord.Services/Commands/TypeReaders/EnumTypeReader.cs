using System.Reflection;

using NetCord.Services.EnumTypeReaders;

namespace NetCord.Services.Commands.TypeReaders;

public class EnumTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    private readonly EnumTypeReaderManager<IEnumTypeReader, CommandParameter<TContext>, CommandParameter<TContext>, CommandServiceConfiguration<TContext>> _enumTypeReaderManager;
    private readonly Dictionary<Type, bool> _byValueTypes = new();

    public unsafe EnumTypeReader()
    {
        _enumTypeReaderManager = new(&GetKey, (parameter, _, configuration) =>
        {
            var type = parameter.NonNullableElementType;
            var allowByValueAttributeType = typeof(AllowByValueAttribute);

            if (parameter.Attributes.ContainsKey(allowByValueAttributeType))
                return new EnumNameOrValueTypeReader(type, configuration.IgnoreCase, configuration.CultureInfo);

            var byValueTypes = _byValueTypes;
            if (!byValueTypes.TryGetValue(type, out var byValueType))
                byValueTypes.Add(type, byValueType = type.IsDefined(typeof(AllowByValueAttribute)));

            return byValueType ? new EnumNameOrValueTypeReader(type, configuration.IgnoreCase, configuration.CultureInfo) : new EnumNameTypeReader(type, configuration.IgnoreCase);
        });

        static CommandParameter<TContext> GetKey(CommandParameter<TContext> parameter) => parameter;
    }

    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (_enumTypeReaderManager.GetTypeReader(parameter, configuration).TryRead(input, out var value))
            return Task.FromResult<object?>(value);

        throw new FormatException($"Invalid {parameter.NonNullableElementType.Name}.");
    }
}
