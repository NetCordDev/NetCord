using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Commands;

public interface ICommandService : IService
{
    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type);

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] T>();

    public void AddCommand(IEnumerable<string> aliases, Delegate handler, int priority = 0);
}
