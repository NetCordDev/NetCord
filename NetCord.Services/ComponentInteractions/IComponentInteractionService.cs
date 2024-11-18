using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ComponentInteractions;

public interface IComponentInteractionService : IService
{
    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type);

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] T>();

    public void AddInteraction(string customId, Delegate handler);
}
