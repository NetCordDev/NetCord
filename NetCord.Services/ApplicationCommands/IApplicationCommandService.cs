using System.Diagnostics.CodeAnalysis;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandService : IService
{
    internal IReadOnlyList<IApplicationCommandInfo> Commands { get; }

    internal void AddRegisteredCommands(IReadOnlyList<RegisteredApplicationCommandInfo> commands);

    /// <summary>
    /// Gets the list of application commands registered in the service.
    /// </summary>
    /// <returns>The list of application commands registered in the service.</returns>
    public IReadOnlyList<IApplicationCommandInfo> GetCommands();

    /// <summary>
    /// Adds a module to the service.
    /// </summary>
    /// <param name="type">The type of the module to add.</param>
    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type);

    /// <summary>
    /// Adds a module to the service.
    /// </summary>
    /// <typeparam name="T">The type of the module to add.</typeparam>
    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>();

    /// <summary>
    /// Adds a slash command to the service.
    /// </summary>
    /// <param name="builder">The slash command builder.</param>
    public void AddSlashCommand(SlashCommandBuilder builder);

    /// <summary>
    /// Adds a slash command group to the service.
    /// </summary>
    /// <param name="builder">The slash command group builder.</param>
    public void AddSlashCommandGroup(SlashCommandGroupBuilder builder);

    /// <summary>
    /// Adds a user command to the service.
    /// </summary>
    /// <param name="builder">The user command builder.</param>
    public void AddUserCommand(UserCommandBuilder builder);

    /// <summary>
    /// Adds a message command to the service.
    /// </summary>
    /// <param name="builder">The message command builder.</param>
    public void AddMessageCommand(MessageCommandBuilder builder);

    /// <summary>
    /// Adds an entry point command to the service.
    /// </summary>
    /// <param name="builder">The entry point command builder.</param>
    public void AddEntryPointCommand(EntryPointCommandBuilder builder);

    /// <inheritdoc cref="ApplicationCommandServiceManager.RegisterCommandsAsync(RestClient, ulong, ulong?, RestRequestProperties?, CancellationToken)" />
    public Task<IReadOnlyList<ApplicationCommand>> RegisterCommandsAsync(RestClient client, ulong applicationId, ulong? guildId = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default);
}
