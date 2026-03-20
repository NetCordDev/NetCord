using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Commands;

/// <summary>
/// Base interface for <see cref="CommandService{TContext}"/>.
/// </summary>
public interface ICommandService : IService
{
    /// <summary>
    /// Adds a command module to the service.
    /// </summary>
    /// <param name="type">The type of the command module to add.</param>
    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type);

    /// <summary>
    /// Adds a command module to the service.
    /// </summary>
    /// <typeparam name="T">The type of the command module to add.</typeparam>
    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>();

    /// <summary>
    /// Adds a command to the service.
    /// </summary>
    /// <param name="builder">The command builder.</param>
    public void AddCommand(CommandBuilder builder);

    /// <summary>
    /// Adds a command group to the service.
    /// </summary>
    /// <param name="builder">The command group builder.</param>
    public void AddCommandGroup(CommandGroupBuilder builder);
}
