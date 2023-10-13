using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandServiceManager
{
    private readonly List<(Action<IReadOnlyList<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)>> AddCommands, IReadOnlyList<IApplicationCommandInfo> CommandInfos)> _globalCommands = [];
    private readonly List<(Action<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)> AddCommand, IReadOnlyList<IApplicationCommandInfo> CommandInfos)> _guildCommands = [];

    public void AddService<TContext>(ApplicationCommandService<TContext> service) where TContext : IApplicationCommandContext
    {
        _globalCommands.Add((service.AddCommands, service._globalCommandsToCreate));
        _guildCommands.Add((service.AddCommand, service._guildCommandsToCreate));
    }

    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RequestProperties? properties = null)
    {
        List<ApplicationCommand> list = new(includeGuildCommands ? _globalCommands.Sum(c => c.CommandInfos.Count) + _guildCommands.Sum(c => c.CommandInfos.Count) : _globalCommands.Sum(c => c.CommandInfos.Count));
        var result = (await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, _globalCommands.Select(c => c.CommandInfos).SelectMany(c => c.Select(x => x.GetRawValue())), properties).ConfigureAwait(false)).Values.Zip(_globalCommands.SelectMany(x => x.CommandInfos)).ToArray();
        int i = 0;
        foreach (var (addCommands, commandInfos) in _globalCommands)
        {
            var count = commandInfos.Count;
            ArraySegment<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)> commands = new(result, i, count);
            i += count;
            addCommands(commands);
            list.AddRange(commands.Select(c => c.Command));
        }

        if (includeGuildCommands)
        {
            foreach (var g in _guildCommands.SelectMany(x => x.CommandInfos.Select(d => (d, x.AddCommand))).GroupBy(c => c.d.GuildId))
            {
                var guildResult = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, g.Key.GetValueOrDefault(), g.Select(c => c.d.GetRawValue()), properties).ConfigureAwait(false);
                foreach (var (first, second) in g.Zip(guildResult.Values))
                {
                    first.AddCommand((second, first.d));
                    list.Add(second);
                }
            }
        }
        return list;
    }
}
