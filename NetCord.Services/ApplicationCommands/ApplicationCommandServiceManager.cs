using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandServiceManager
{
    public ApplicationCommandServiceManager() : this([])
    {
    }

    internal ApplicationCommandServiceManager(List<IApplicationCommandService> services)
    {
        _services = services;
    }

    private readonly List<IApplicationCommandService> _services;

    public IReadOnlyList<IApplicationCommandService> Services => _services;

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
            .Select(service =>
            {
                var commands = service.Commands.Where(c => c.Register).ToArray();
                int length = commands.Length;
                return (Service: service, Commands: commands, RegisteredCommands: length is 0 ? [] : new RegisteredApplicationCommandInfo[length]);
            })
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
            registeredCommands[index.Value++] = new(command, commandInfo);

        foreach (var (service, _, registeredCommands) in serviceCommands)
            service.AddRegisteredCommands(registeredCommands);

        return createdCommands;
    }
}
