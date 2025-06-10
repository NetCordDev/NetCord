using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandService : IService
{
    internal IReadOnlyList<IApplicationCommandInfo> GlobalCommands { get; }

    internal IEnumerable<GuildCommands> GuildCommands { get; }

    internal void SetCommands(IEnumerable<KeyValuePair<ulong, IApplicationCommandInfo>> commands);

    internal int GetApproximateCommandsCount(bool includeGuildCommands);

    /// <summary>
    /// Adds a module to the service.
    /// </summary>
    /// <param name="type">Type of the module.</param>
    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type);

    /// <summary>
    /// Adds a module to the service.
    /// </summary>
    /// <typeparam name="T">The module.</typeparam>
    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>();

    /// <summary>
    /// Adds a slash command to the service.
    /// </summary>
    /// <param name="name"><inheritdoc cref="ApplicationCommandAttribute.Name" path="/summary" /> Must be lowercase.</param>
    /// <param name="description"><inheritdoc cref="SlashCommandAttribute.Description" path="/summary" /></param>
    /// <param name="handler">Handler for the command.</param>
    /// <param name="defaultGuildUserPermissions"><inheritdoc cref="ApplicationCommandAttribute.DefaultGuildUserPermissions" path="/summary" /></param>
    /// <param name="dMPermission"><inheritdoc cref="ApplicationCommandAttribute.DMPermission" path="/summary" /></param>
    /// <param name="defaultPermission"><inheritdoc cref="ApplicationCommandAttribute.DefaultPermission" path="/summary" /></param>
    /// <param name="integrationTypes"><inheritdoc cref="ApplicationCommandAttribute.IntegrationTypes" path="/summary" /></param>
    /// <param name="contexts"><inheritdoc cref="ApplicationCommandAttribute.Contexts" path="/summary" /></param>
    /// <param name="nsfw"><inheritdoc cref="ApplicationCommandAttribute.Nsfw" path="/summary" /></param>/>
    /// <param name="guildId"><inheritdoc cref="ApplicationCommandAttribute.GuildId" path="/summary" /></param>
    public void AddSlashCommand(string name,
                                string description,
                                Delegate handler,
                                Permissions? defaultGuildUserPermissions = null,
                                bool? dMPermission = null,
                                bool defaultPermission = true,
                                IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                IEnumerable<InteractionContextType>? contexts = null,
                                bool nsfw = false,
                                ulong? guildId = null);

    /// <summary>
    /// Adds a nested slash command to the service.
    /// </summary>
    /// <param name="name"><inheritdoc cref="ApplicationCommandAttribute.Name" path="/summary" /> Must be lowercase.</param>
    /// <param name="description"><inheritdoc cref="SlashCommandAttribute.Description" path="/summary" /></param>
    /// <param name="builder">Builder for the command.</param>
    /// <param name="defaultGuildUserPermissions"><inheritdoc cref="ApplicationCommandAttribute.DefaultGuildUserPermissions" path="/summary" /></param>
    /// <param name="dMPermission"><inheritdoc cref="ApplicationCommandAttribute.DMPermission" path="/summary" /></param>
    /// <param name="defaultPermission"><inheritdoc cref="ApplicationCommandAttribute.DefaultPermission" path="/summary" /></param>
    /// <param name="integrationTypes"><inheritdoc cref="ApplicationCommandAttribute.IntegrationTypes" path="/summary" /></param>
    /// <param name="contexts"><inheritdoc cref="ApplicationCommandAttribute.Contexts" path="/summary" /></param>
    /// <param name="nsfw"><inheritdoc cref="ApplicationCommandAttribute.Nsfw" path="/summary" /></param>/>
    /// <param name="guildId"><inheritdoc cref="ApplicationCommandAttribute.GuildId" path="/summary" /></param>
    public void AddSlashCommand(string name,
                                string description,
                                Action<SlashCommandBuilder> builder,
                                Permissions? defaultGuildUserPermissions = null,
                                bool? dMPermission = null,
                                bool defaultPermission = true,
                                IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                IEnumerable<InteractionContextType>? contexts = null,
                                bool nsfw = false,
                                ulong? guildId = null);

    /// <summary>
    /// Adds a user command to the service.
    /// </summary>
    /// <param name="name"><inheritdoc cref="ApplicationCommandAttribute.Name" path="/summary" /> Must be lowercase.</param>
    /// <param name="handler">Handler for the command.</param>
    /// <param name="defaultGuildUserPermissions"><inheritdoc cref="ApplicationCommandAttribute.DefaultGuildUserPermissions" path="/summary" /></param>
    /// <param name="dMPermission"><inheritdoc cref="ApplicationCommandAttribute.DMPermission" path="/summary" /></param>
    /// <param name="defaultPermission"><inheritdoc cref="ApplicationCommandAttribute.DefaultPermission" path="/summary" /></param>
    /// <param name="integrationTypes"><inheritdoc cref="ApplicationCommandAttribute.IntegrationTypes" path="/summary" /></param>
    /// <param name="contexts"><inheritdoc cref="ApplicationCommandAttribute.Contexts" path="/summary" /></param>
    /// <param name="nsfw"><inheritdoc cref="ApplicationCommandAttribute.Nsfw" path="/summary" /></param>/>
    /// <param name="guildId"><inheritdoc cref="ApplicationCommandAttribute.GuildId" path="/summary" /></param>
    public void AddUserCommand(string name,
                               Delegate handler,
                               Permissions? defaultGuildUserPermissions = null,
                               bool? dMPermission = null,
                               bool defaultPermission = true,
                               IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                               IEnumerable<InteractionContextType>? contexts = null,
                               bool nsfw = false,
                               ulong? guildId = null);

    /// <summary>
    /// Adds a message command to the service.
    /// </summary>
    /// <param name="name"><inheritdoc cref="ApplicationCommandAttribute.Name" path="/summary" /> Must be lowercase.</param>
    /// <param name="handler">Handler for the command.</param>
    /// <param name="defaultGuildUserPermissions"><inheritdoc cref="ApplicationCommandAttribute.DefaultGuildUserPermissions" path="/summary" /></param>
    /// <param name="dMPermission"><inheritdoc cref="ApplicationCommandAttribute.DMPermission" path="/summary" /></param>
    /// <param name="defaultPermission"><inheritdoc cref="ApplicationCommandAttribute.DefaultPermission" path="/summary" /></param>
    /// <param name="integrationTypes"><inheritdoc cref="ApplicationCommandAttribute.IntegrationTypes" path="/summary" /></param>
    /// <param name="contexts"><inheritdoc cref="ApplicationCommandAttribute.Contexts" path="/summary" /></param>
    /// <param name="nsfw"><inheritdoc cref="ApplicationCommandAttribute.Nsfw" path="/summary" /></param>/>
    /// <param name="guildId"><inheritdoc cref="ApplicationCommandAttribute.GuildId" path="/summary" /></param>
    public void AddMessageCommand(string name,
                                  Delegate handler,
                                  Permissions? defaultGuildUserPermissions = null,
                                  bool? dMPermission = null,
                                  bool defaultPermission = true,
                                  IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                  IEnumerable<InteractionContextType>? contexts = null,
                                  bool nsfw = false,
                                  ulong? guildId = null);

    /// <summary>
    /// Adds an entry point command to the service.
    /// </summary>
    /// <param name="name"><inheritdoc cref="ApplicationCommandAttribute.Name" path="/summary" /> Must be lowercase.</param>
    /// <param name="description"><inheritdoc cref="EntryPointCommandAttribute.Description" path="/summary" /></param>
    /// <param name="handler">Handler for the command. If <see langword="null"/>, Discord will handle the interaction automatically by launching the associated Activity and sending a message to the channel where it was launched.</param>
    /// <param name="defaultGuildUserPermissions"><inheritdoc cref="ApplicationCommandAttribute.DefaultGuildUserPermissions" path="/summary" /></param>
    /// <param name="dMPermission"><inheritdoc cref="ApplicationCommandAttribute.DMPermission" path="/summary" /></param>
    /// <param name="defaultPermission"><inheritdoc cref="ApplicationCommandAttribute.DefaultPermission" path="/summary" /></param>
    /// <param name="integrationTypes"><inheritdoc cref="ApplicationCommandAttribute.IntegrationTypes" path="/summary" /></param>
    /// <param name="contexts"><inheritdoc cref="ApplicationCommandAttribute.Contexts" path="/summary" /></param>
    /// <param name="nsfw"><inheritdoc cref="ApplicationCommandAttribute.Nsfw" path="/summary" /></param>/>
    /// <param name="guildId"><inheritdoc cref="ApplicationCommandAttribute.GuildId" path="/summary" /></param>
    public void AddEntryPointCommand(string name,
                                     string description,
                                     Delegate? handler = null,
                                     Permissions? defaultGuildUserPermissions = null,
                                     bool? dMPermission = null,
                                     bool defaultPermission = true,
                                     IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                     IEnumerable<InteractionContextType>? contexts = null,
                                     bool nsfw = false,
                                     ulong? guildId = null);
}
