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

        var globalInfos = services.SelectMany(s => s.GlobalCommands.Select(c => (Service: s, Command: c))).ToArray();

        List<ApplicationCommand> list = new(globalInfos.Length);

        var globalProperties = globalInfos.Select(c => c.Command.GetRawValue());
        var created = await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, globalProperties, properties).ConfigureAwait(false);

        foreach (var (command, (service, commandInfo)) in created.Zip(globalInfos))
        {
            service.AddCommand(command.Key, commandInfo);
            list.Add(command.Value);
        }

        if (includeGuildCommands)
        {
            foreach (var guildCommandsGroup in services.SelectMany(s => s.GuildCommands.Select(c => (Service: s, Commands: c))).GroupBy(c => c.Commands.GuildId))
            {
                var guildInfos = guildCommandsGroup.SelectMany(g => g.Commands.Commands.Select(c => (g.Service, Command: c))).ToArray();

                var guildProperties = guildInfos.Select(c => c.Command.GetRawValue());

                var guildCreated = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, guildCommandsGroup.Key, guildProperties, properties).ConfigureAwait(false);

                foreach (var (command, (service, commandInfo)) in guildCreated.Zip(guildInfos))
                {
                    service.AddCommand(command.Key, commandInfo);
                    list.Add(command.Value);
                }
            }
        }
        return list;
    }
}
