namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandService
{
    internal IReadOnlyList<IApplicationCommandInfo> GlobalCommands { get; }

    internal IEnumerable<GuildCommands> GuildCommands { get; }

    internal void SetCommands(IEnumerable<KeyValuePair<ulong, IApplicationCommandInfo>> commands);

    internal int GetApproximateCommandsCount(bool includeGuildCommands);
}
