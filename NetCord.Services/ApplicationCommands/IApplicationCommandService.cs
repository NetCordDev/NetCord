using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandService : IService
{
    internal IReadOnlyList<IApplicationCommandInfo> GlobalCommands { get; }

    internal IEnumerable<GuildCommands> GuildCommands { get; }

    internal void SetCommands(IEnumerable<KeyValuePair<ulong, IApplicationCommandInfo>> commands);

    internal int GetApproximateCommandsCount(bool includeGuildCommands);

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type);

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>();

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

    public void AddUserCommand(string name,
                               Delegate handler,
                               Permissions? defaultGuildUserPermissions = null,
                               bool? dMPermission = null,
                               bool defaultPermission = true,
                               IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                               IEnumerable<InteractionContextType>? contexts = null,
                               bool nsfw = false,
                               ulong? guildId = null);

    public void AddMessageCommand(string name,
                                  Delegate handler,
                                  Permissions? defaultGuildUserPermissions = null,
                                  bool? dMPermission = null,
                                  bool defaultPermission = true,
                                  IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                  IEnumerable<InteractionContextType>? contexts = null,
                                  bool nsfw = false,
                                  ulong? guildId = null);
}
