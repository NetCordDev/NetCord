namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandService
{
    internal IReadOnlyList<IApplicationCommandInfo> GlobalCommands { get; }

    internal IEnumerable<GuildCommands> GuildCommands { get; }

    internal void AddCommand(ulong id, IApplicationCommandInfo applicationCommandInfo);
}
