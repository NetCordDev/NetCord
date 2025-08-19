using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandService : IService
{
    internal IReadOnlyList<IApplicationCommandInfo> Commands { get; }

    internal void AddRegisteredCommands(IReadOnlyList<RegisteredApplicationCommandInfo> commands);

    public IReadOnlyList<IApplicationCommandInfo> GetCommands();

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
                                bool register = true);

    public void AddSlashCommand(string name,
                                string description,
                                Action<SlashCommandBuilder> builder,
                                Permissions? defaultGuildUserPermissions = null,
                                bool? dMPermission = null,
                                bool defaultPermission = true,
                                IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                IEnumerable<InteractionContextType>? contexts = null,
                                bool nsfw = false,
                                bool register = true);

    public void AddUserCommand(string name,
                               Delegate handler,
                               Permissions? defaultGuildUserPermissions = null,
                               bool? dMPermission = null,
                               bool defaultPermission = true,
                               IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                               IEnumerable<InteractionContextType>? contexts = null,
                               bool nsfw = false,
                               bool register = true);

    public void AddMessageCommand(string name,
                                  Delegate handler,
                                  Permissions? defaultGuildUserPermissions = null,
                                  bool? dMPermission = null,
                                  bool defaultPermission = true,
                                  IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                  IEnumerable<InteractionContextType>? contexts = null,
                                  bool nsfw = false,
                                  bool register = true);

    public void AddEntryPointCommand(string name,
                                     string description,
                                     Delegate? handler = null,
                                     Permissions? defaultGuildUserPermissions = null,
                                     bool? dMPermission = null,
                                     bool defaultPermission = true,
                                     IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                     IEnumerable<InteractionContextType>? contexts = null,
                                     bool nsfw = false,
                                     bool register = true);
}
