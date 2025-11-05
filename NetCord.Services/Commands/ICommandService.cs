using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Commands;

public interface ICommandService : IService
{
    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type);

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>();

    public void AddCommand(CommandBuilder builder);

    public void AddCommandGroup(CommandGroupBuilder builder);
}
