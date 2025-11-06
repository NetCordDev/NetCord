using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Allows registration of application commands from multiple instances of <see cref="ApplicationCommandService{TContext}"/>.
/// </summary>
public class ApplicationCommandServiceManager
{
    /// <inheritdoc cref="ApplicationCommandServiceManager" />
    public ApplicationCommandServiceManager() : this([])
    {
    }

    internal ApplicationCommandServiceManager(List<IApplicationCommandService> services)
    {
        _services = services;
    }

    private readonly List<IApplicationCommandService> _services;

    /// <summary>
    /// The application command services added to the manager.
    /// </summary>
    public IReadOnlyList<IApplicationCommandService> Services => _services;

    /// <summary>
    /// Adds an application command service to the manager.
    /// </summary>
    /// <param name="service">The application command service to add.</param>
    public void AddService(IApplicationCommandService service)
    {
        _services.Add(service);
    }

    /// <summary>
    /// Registers the application commands to Discord.
    /// </summary>
    /// <param name="client">The <see cref="RestClient"/> to use for registration.</param>
    /// <param name="applicationId">The application ID.</param>
    /// <param name="guildId">The guild ID for guild-specific commands, or <see langword="null"/> for global commands.</param>
    /// <param name="properties">The <see cref="RestClient"/>'s request properties to use for registration.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task representing the registered application commands.</returns>
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
