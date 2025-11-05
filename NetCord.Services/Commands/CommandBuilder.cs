namespace NetCord.Services.Commands;

[GenerateMethodsForProperties]
public partial interface ICommandBuilder
{
    public IEnumerable<string> Aliases { get; }

    public int Priority { get; set; }
}

[GenerateMethodsForProperties]
public partial class CommandBuilder(IEnumerable<string> aliases, Delegate handler) : ICommandBuilder
{
    public IEnumerable<string> Aliases => aliases;

    public Delegate Handler => handler;

    public int Priority { get; set; }
}

[GenerateMethodsForProperties]
public partial class CommandGroupBuilder(IEnumerable<string> aliases) : ICommandBuilder
{
    internal readonly List<CommandBuilder> _subCommandBuilders = [];
    internal readonly List<CommandGroupBuilder> _subCommandGroupBuilders = [];

    public IEnumerable<string> Aliases => aliases;

    public int Priority { get; set; }

    public CommandBuilder AddSubCommand(IEnumerable<string> aliases, Delegate handler)
    {
        CommandBuilder result = new(aliases, handler);
        _subCommandBuilders.Add(result);
        return result;
    }

    public CommandGroupBuilder AddSubCommandGroup(IEnumerable<string> aliases)
    {
        CommandGroupBuilder result = new(aliases);
        _subCommandGroupBuilders.Add(result);
        return result;
    }

    public CommandGroupBuilder AddSubCommandGroup(IEnumerable<string> aliases, Action<CommandGroupBuilder> builder)
    {
        CommandGroupBuilder result = new(aliases);
        builder(result);
        _subCommandGroupBuilders.Add(result);
        return result;
    }
}
