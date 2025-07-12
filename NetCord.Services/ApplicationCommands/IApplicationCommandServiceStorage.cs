using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandServiceStorage<TContext> where TContext : IApplicationCommandContext
{
    public void AddCommand(ApplicationCommandInfo<TContext> command);

    public void AddRegisteredCommands(IEnumerable<RegisteredApplicationCommandInfo<TContext>> registeredCommands);

    public bool TryGetCommand(ApplicationCommandInteractionData interactionData, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> command);
}

public class IdApplicationCommandServiceStorage<TContext> : IApplicationCommandServiceStorage<TContext> where TContext : IApplicationCommandContext
{
    private FrozenDictionary<ulong, ApplicationCommandInfo<TContext>> _commands = FrozenDictionary<ulong, ApplicationCommandInfo<TContext>>.Empty;

    public void AddCommand(ApplicationCommandInfo<TContext> command)
    {
    }

    public void AddRegisteredCommands(IEnumerable<RegisteredApplicationCommandInfo<TContext>> registeredCommands)
    {
        if (_commands.Count != 0)
            ThrowCommandsAlreadyRegistered();

        _commands = registeredCommands.ToFrozenDictionary(c => c.CommandId, c => c.CommandInfo);
    }

    [DoesNotReturn]
    private static void ThrowCommandsAlreadyRegistered()
    {
        throw new InvalidOperationException($"'{nameof(IdApplicationCommandServiceStorage<>)}' does not support registering application commands more than once. Consider using other storage options like '{nameof(NameApplicationCommandServiceStorage<>)}'.");
    }

    public bool TryGetCommand(ApplicationCommandInteractionData interactionData, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> command)
    {
        return _commands.TryGetValue(interactionData.Id, out command);
    }
}

public class NameApplicationCommandServiceStorage<TContext> : IApplicationCommandServiceStorage<TContext> where TContext : IApplicationCommandContext
{
    private readonly Dictionary<string, ApplicationCommandInfo<TContext>> _commands = [];

    public void AddCommand(ApplicationCommandInfo<TContext> command)
    {
        _commands.Add(command.Name, command);
    }

    public void AddRegisteredCommands(IEnumerable<RegisteredApplicationCommandInfo<TContext>> registeredCommands)
    {
    }

    public bool TryGetCommand(ApplicationCommandInteractionData interactionData, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> command)
    {
        return _commands.TryGetValue(interactionData.Name, out command);
    }
}
