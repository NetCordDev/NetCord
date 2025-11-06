using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ComponentInteractions;

/// <summary>
/// Base interface for <see cref="ComponentInteractionService{TContext}"/>.
/// </summary>
public interface IComponentInteractionService : IService
{
    /// <summary>
    /// Adds a module to the service.
    /// </summary>
    /// <param name="type">The type of the module to add.</param>
    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type);

    /// <summary>
    /// Adds a module to the service.
    /// </summary>
    /// <typeparam name="T">The type of the module to add.</typeparam>
    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] T>();

    /// <summary>
    /// Adds a component interaction to the service.
    /// </summary>
    /// <param name="builder">The component interaction builder.</param>
    public void AddComponentInteraction(ComponentInteractionBuilder builder);
}
