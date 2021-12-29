namespace NetCord.Commands;

[AttributeUsage(AttributeTargets.Parameter)]
public class TypeReaderAttribute<TTypeReader, TContext> : Attribute where TTypeReader : ITypeReader<TContext>, new() where TContext : ICommandContext
{
    public CommandServiceOptions<TContext>.TypeReader ReadAsync { get; }

    public TypeReaderAttribute()
    {
        ReadAsync = new TTypeReader().ReadAsync;
    }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class TypeReaderAttribute<TTypeReader> : TypeReaderAttribute<TTypeReader, CommandContext> where TTypeReader : ITypeReader<CommandContext>, new()
{
}