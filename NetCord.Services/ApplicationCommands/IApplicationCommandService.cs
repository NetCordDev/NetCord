using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandService : IService
{
    internal IReadOnlyList<IApplicationCommandInfo> Commands { get; }

    internal void AddRegisteredCommands(IReadOnlyList<RegisteredApplicationCommandInfo> commands);

    public IReadOnlyList<IApplicationCommandInfo> GetCommands();

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type);

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>();

    public void AddSlashCommand(SlashCommandBuilder builder);

    public void AddSlashCommandGroup(SlashCommandGroupBuilder builder);

    public void AddUserCommand(UserCommandBuilder builder);

    public void AddMessageCommand(MessageCommandBuilder builder);

    public void AddEntryPointCommand(EntryPointCommandBuilder builder);
}
