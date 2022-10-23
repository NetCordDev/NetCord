using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandServiceManager
{
    private readonly List<(Action<IEnumerable<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)>> Action, IReadOnlyList<IApplicationCommandInfo> CommandInfos)> _globalCommands = new();
    private readonly List<(Action<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)> Action, IReadOnlyList<IApplicationCommandInfo> CommandInfos)> _guildCommands = new();

    public void AddService<TContext>(ApplicationCommandService<TContext> service) where TContext : IApplicationCommandContext
    {
        _globalCommands.Add((service.AddCommands, service._globalCommandsToCreate));
        _guildCommands.Add((service.AddCommand, service._guildCommandsToCreate));
    }

    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RequestProperties? properties = null)
    {
        List<ApplicationCommand> list = new(_globalCommands.Sum(c => c.CommandInfos.Count) + _guildCommands.Sum(c => c.CommandInfos.Count));
        var result = (await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, _globalCommands.Select(c => c.CommandInfos).SelectMany(c => c.Select(x => x.GetRawValue())), properties).ConfigureAwait(false)).Values.Zip(_globalCommands.SelectMany(x => x.CommandInfos)).ToArray();
        int i = 0;
        foreach (var s in _globalCommands)
        {
            var commands = result[i..(i += s.CommandInfos.Count)];
            s.Action(commands);
            list.AddRange(commands.Select(c => c.First));
        }

        if (includeGuildCommands)
        {
            foreach (var g in _guildCommands.SelectMany(x => x.CommandInfos.Select(d => (d, x.Action))).GroupBy(c => c.d.GuildId))
            {
                var guildResult = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, g.Key.GetValueOrDefault(), g.Select(c => c.d.GetRawValue()), properties).ConfigureAwait(false);
                foreach (var (First, Second) in g.Zip(guildResult.Values))
                {
                    First.Action((Second, First.d));
                    list.Add(Second);
                }
            }
        }
        return list;
    }
}
