using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandServiceManager
{
    private readonly List<IApplicationCommandService> _services = [];

    public void AddService(IApplicationCommandService service)
    {
        _services.Add(service);
    }

    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RequestProperties? properties = null)
    {
        var services = _services.ToArray();
        var serviceCommands = services.Select(s => (Service: s, Commands: new List<KeyValuePair<ulong, IApplicationCommandInfo>>(s.GetApproximateCommandsCount(includeGuildCommands)))).ToArray();

        var globalInfos = serviceCommands.SelectMany(serviceCommands => serviceCommands.Service.GlobalCommands.Select(command => (serviceCommands.Service, serviceCommands.Commands, Command: command))).ToArray();

        List<ApplicationCommand> result = new(globalInfos.Length);

        var globalProperties = globalInfos.Select(c => c.Command.GetRawValue());
        var created = await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, globalProperties, properties).ConfigureAwait(false);

        foreach (var (command, (service, commands, commandInfo)) in created.Zip(globalInfos))
        {
            commands.Add(new(command.Key, commandInfo));
            result.Add(command.Value);
        }

        if (includeGuildCommands)
        {
            foreach (var guildCommandsGroup in serviceCommands.SelectMany(serviceCommands => serviceCommands.Service.GuildCommands.Select(c => (serviceCommands.Service, serviceCommands.Commands, GuildCommands: c))).GroupBy(c => c.GuildCommands.GuildId))
            {
                var guildInfos = guildCommandsGroup.SelectMany(g => g.GuildCommands.Commands.Select(c => (g.Service, g.Commands, Command: c))).ToArray();

                var guildProperties = guildInfos.Select(c => c.Command.GetRawValue());

                var guildCreated = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, guildCommandsGroup.Key, guildProperties, properties).ConfigureAwait(false);

                foreach (var (command, (service, commands, commandInfo)) in guildCreated.Zip(guildInfos))
                {
                    commands.Add(new(command.Key, commandInfo));
                    result.Add(command.Value);
                }
            }
        }

        foreach (var (service, commands) in serviceCommands)
            service.SetCommands(commands);

        return result;
    }
}
