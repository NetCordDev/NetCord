using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandServiceManager
{
    private readonly List<IApplicationCommandService> _services = [];

    public void AddService(IApplicationCommandService service)
    {
        _services.Add(service);
    }

    public Task<IReadOnlyList<ApplicationCommand>> RegisterCommandsAsync(RestClient client, ulong applicationId, ulong? guildId = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return RegisterCommandsAsync(_services, client, applicationId, guildId, properties, cancellationToken);
    }

    internal static async Task<IReadOnlyList<ApplicationCommand>> RegisterCommandsAsync(IReadOnlyList<IApplicationCommandService> services, RestClient client, ulong applicationId, ulong? guildId, RestRequestProperties? properties, CancellationToken cancellationToken)
    {
        var serviceCommands = services
            .Select(service => (Service: service, Commands: service.Commands.Where(c => c.Register).ToArray()))
            .Where(commands => commands.Commands.Length is not 0)
            .Select(commands => (commands.Service, commands.Commands, RegisteredCommands: new KeyValuePair<ulong, IApplicationCommandInfo>[commands.Commands.Length]))
            .ToArray();

        var commandsToCreate = serviceCommands
            .SelectMany(commands =>
            {
                StrongBox<int> index = new();
                return commands.Commands.Select(command => (Index: index, commands.RegisteredCommands, Command: command));
            })
            .ToArray();

        int count = commandsToCreate.Length;
        var commandProperties = new ApplicationCommandProperties[count];

        for (int i = 0; i < count; i++)
            commandProperties[i] = await commandsToCreate[i].Command.GetRawValueAsync(cancellationToken).ConfigureAwait(false);

        var createdCommands = guildId.HasValue
            ? await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, guildId.GetValueOrDefault(), commandProperties, properties, cancellationToken).ConfigureAwait(false)
            : await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, commandProperties, properties, cancellationToken).ConfigureAwait(false);

        foreach (var (command, (index, registeredCommands, commandInfo)) in createdCommands.Zip(commandsToCreate))
            registeredCommands[index.Value++] = new(command.Id, commandInfo);

        foreach (var (service, _, registeredCommands) in serviceCommands)
            service.AddCommands(registeredCommands);

        return createdCommands;
    }
}
