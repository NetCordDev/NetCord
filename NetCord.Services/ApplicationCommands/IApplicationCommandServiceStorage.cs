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
    private byte _registered;

    public void AddCommand(ApplicationCommandInfo<TContext> command)
    {
    }

    public void AddRegisteredCommands(IEnumerable<RegisteredApplicationCommandInfo<TContext>> registeredCommands)
    {
        if (Interlocked.CompareExchange(ref _registered, 1, 0) is 1)
            ThrowCommandsAlreadyRegistered();

        _commands = registeredCommands.ToFrozenDictionary(c => c.CommandId, c => c.CommandInfo);
    }

    [DoesNotReturn]
    private static void ThrowCommandsAlreadyRegistered()
    {
        throw new InvalidOperationException($"'{nameof(IdApplicationCommandServiceStorage<>)}' does not support registering application commands more than once. Consider using other storage options like '{nameof(NameAndTypeApplicationCommandServiceStorage<>)}'.");
    }

    public bool TryGetCommand(ApplicationCommandInteractionData interactionData, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> command)
    {
        return _commands.TryGetValue(interactionData.Id, out command);
    }
}

public class NameAndTypeApplicationCommandServiceStorage<TContext> : IApplicationCommandServiceStorage<TContext> where TContext : IApplicationCommandContext
{
    private readonly Dictionary<Key, ApplicationCommandInfo<TContext>> _commands = [];

    internal readonly record struct Key(string Name, ApplicationCommandType Type);

    public void AddCommand(ApplicationCommandInfo<TContext> command)
    {
        _commands.Add(new(command.Name, command.Type), command);
    }

    public void AddRegisteredCommands(IEnumerable<RegisteredApplicationCommandInfo<TContext>> registeredCommands)
    {
    }

    public bool TryGetCommand(ApplicationCommandInteractionData interactionData, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> command)
    {
        return _commands.TryGetValue(new(interactionData.Name, interactionData.Type), out command);
    }
}
