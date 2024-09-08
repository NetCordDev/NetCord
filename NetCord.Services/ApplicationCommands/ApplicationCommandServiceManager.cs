using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandServiceManager
{
    private readonly List<IApplicationCommandService> _services = [];

    public void AddService(IApplicationCommandService service)
    {
        _services.Add(service);
    }

    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        var services = _services.ToArray();
        var serviceCommands = services.Select(s => (Service: s, Commands: new List<KeyValuePair<ulong, IApplicationCommandInfo>>(s.GetApproximateCommandsCount(includeGuildCommands)))).ToArray();

        var globalInfos = serviceCommands.SelectMany(serviceCommands => serviceCommands.Service.GlobalCommands.Select(command => (serviceCommands.Service, serviceCommands.Commands, Command: command))).ToArray();

        int globalLength = globalInfos.Length;
        var globalProperties = new ApplicationCommandProperties[globalLength];

        for (int i = 0; i < globalLength; i++)
            globalProperties[i] = await globalInfos[i].Command.GetRawValueAsync(cancellationToken).ConfigureAwait(false);

        var created = await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, globalProperties, properties, cancellationToken).ConfigureAwait(false);

        List<ApplicationCommand> result = new(globalLength);

        foreach (var (command, (service, commands, commandInfo)) in created.Zip(globalInfos))
        {
            commands.Add(new(command.Id, commandInfo));
            result.Add(command);
        }

        if (includeGuildCommands)
        {
            foreach (var guildCommandsGroup in serviceCommands.SelectMany(serviceCommands => serviceCommands.Service.GuildCommands.Select(c => (serviceCommands.Service, serviceCommands.Commands, GuildCommands: c))).GroupBy(c => c.GuildCommands.GuildId))
            {
                var guildInfos = guildCommandsGroup.SelectMany(g => g.GuildCommands.Commands.Select(c => (g.Service, g.Commands, Command: c))).ToArray();

                var guildLength = guildInfos.Length;
                var guildProperties = new ApplicationCommandProperties[guildLength];

                for (int i = 0; i < guildLength; i++)
                    guildProperties[i] = await guildInfos[i].Command.GetRawValueAsync(cancellationToken).ConfigureAwait(false);

                var guildCreated = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, guildCommandsGroup.Key, guildProperties, properties, cancellationToken).ConfigureAwait(false);

                foreach (var (command, (service, commands, commandInfo)) in guildCreated.Zip(guildInfos))
                {
                    commands.Add(new(command.Id, commandInfo));
                    result.Add(command);
                }
            }
        }

        foreach (var (service, commands) in serviceCommands)
            service.SetCommands(commands);

        return result;
    }
}
