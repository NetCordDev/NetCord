namespace NetCord.Services.Commands;

/// <inheritdoc cref="CommandAttribute" />
[GenerateMethodsForProperties]
public partial interface ICommandBuilder
{
    /// <inheritdoc cref="CommandAttribute.Aliases" />
    public IEnumerable<string> Aliases { get; }

    /// <inheritdoc cref="CommandAttribute.Priority" />
    public int Priority { get; set; }
}

/// <inheritdoc cref="CommandAttribute" />
/// <param name="aliases" />
/// <param name="handler"><inheritdoc cref="Handler" path="/summary" /></param>
[GenerateMethodsForProperties]
public partial class CommandBuilder(IEnumerable<string> aliases, Delegate handler) : ICommandBuilder
{
    /// <inheritdoc cref="CommandAttribute.Aliases" />
    public IEnumerable<string> Aliases => aliases;

    /// <summary>
    /// Handler that represents the body of the command.
    /// </summary>
    public Delegate Handler => handler;

    /// <inheritdoc cref="CommandAttribute.Priority" />
    public int Priority { get; set; }
}

/// <inheritdoc cref="CommandAttribute" />
/// <param name="aliases" />
[GenerateMethodsForProperties]
public partial class CommandGroupBuilder(IEnumerable<string> aliases) : ICommandBuilder
{
    internal readonly List<CommandBuilder> _subCommandBuilders = [];
    internal readonly List<CommandGroupBuilder> _subCommandGroupBuilders = [];

    /// <inheritdoc cref="CommandAttribute.Aliases" />
    public IEnumerable<string> Aliases => aliases;

    /// <inheritdoc cref="CommandAttribute.Priority" />
    public int Priority { get; set; }

    /// <summary>
    /// Adds a sub command to a command group.
    /// </summary>
    /// <param name="aliases"><inheritdoc cref="CommandBuilder.Aliases" path="/summary" /></param>
    /// <param name="handler"><inheritdoc cref="CommandBuilder.Handler" path="/summary" /></param>
    /// <returns>The created sub command builder.</returns>
    public CommandBuilder AddSubCommand(IEnumerable<string> aliases, Delegate handler)
    {
        CommandBuilder result = new(aliases, handler);
        _subCommandBuilders.Add(result);
        return result;
    }

    /// <summary>
    /// Adds a sub command group to a command group.
    /// </summary>
    /// <param name="aliases"><inheritdoc cref="CommandBuilder.Aliases" path="/summary" /></param>
    /// <returns>The created sub command group builder.</returns>
    public CommandGroupBuilder AddSubCommandGroup(IEnumerable<string> aliases)
    {
        CommandGroupBuilder result = new(aliases);
        _subCommandGroupBuilders.Add(result);
        return result;
    }

    /// <inheritdoc cref="AddSubCommandGroup(IEnumerable{string})" />
    /// <param name="aliases" />
    /// <param name="builder">A delegate that builds the sub command group.</param>
    /// <returns>The created sub command group builder.</returns>
    public CommandGroupBuilder AddSubCommandGroup(IEnumerable<string> aliases, Action<CommandGroupBuilder> builder)
    {
        CommandGroupBuilder result = new(aliases);
        builder(result);
        _subCommandGroupBuilders.Add(result);
        return result;
    }
}
