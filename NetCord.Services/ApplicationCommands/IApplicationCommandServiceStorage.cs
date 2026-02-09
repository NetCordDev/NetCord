using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandServiceStorage<TContext> where TContext : IApplicationCommandContext
{
    public void AddCommand(ApplicationCommandInfo<TContext> command);

    public void AddRegisteredCommands(IReadOnlyList<RegisteredApplicationCommandInfo<TContext>> registeredCommands);

    public bool TryGetCommand(ApplicationCommandInteractionData interactionData, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> command);
}

public class IdApplicationCommandServiceStorage<TContext> : IApplicationCommandServiceStorage<TContext> where TContext : IApplicationCommandContext
{
    private FrozenDictionary<ulong, ApplicationCommandInfo<TContext>> _commands = FrozenDictionary<ulong, ApplicationCommandInfo<TContext>>.Empty;
    private byte _registered;

    public IReadOnlyList<RegisteredApplicationCommand<TContext>> GetRegisteredCommands() => [.. _commands.Select(p => new RegisteredApplicationCommand<TContext>(p.Key, p.Value))];

    public void AddCommand(ApplicationCommandInfo<TContext> command)
    {
    }

    public void AddRegisteredCommands(IReadOnlyList<RegisteredApplicationCommandInfo<TContext>> registeredCommands)
    {
        if (Interlocked.Exchange(ref _registered, 1) is 1)
            ThrowCommandsAlreadyRegistered();

        _commands = registeredCommands.ToFrozenDictionary(c => c.Command.Id, c => c.CommandInfo);
    }

    [DoesNotReturn]
    [StackTraceHidden]
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
    private readonly record struct Key(string Name, ApplicationCommandType Type);

    private readonly Dictionary<Key, ApplicationCommandInfo<TContext>> _commands = [];

    public IReadOnlyList<ApplicationCommandInfo<TContext>> GetCommands() => [.. _commands.Values];

    public void AddCommand(ApplicationCommandInfo<TContext> command)
    {
        _commands.Add(new(command.Name, command.Type), command);
    }

    public void AddRegisteredCommands(IReadOnlyList<RegisteredApplicationCommandInfo<TContext>> registeredCommands)
    {
    }

    public bool TryGetCommand(ApplicationCommandInteractionData interactionData, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> command)
    {
        return _commands.TryGetValue(new(interactionData.Name, interactionData.Type), out command);
    }
}
