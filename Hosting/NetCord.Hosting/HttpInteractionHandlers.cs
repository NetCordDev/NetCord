namespace NetCord.Hosting;

public interface IHttpInteractionHandler
{
    /// <summary>
    /// Handles HTTP <see cref="Interaction"/>s.
    /// </summary>
    /// <param name="interaction">The <see cref="Interaction"/> received.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask HandleAsync(Interaction interaction);
}

internal abstract class HttpInteractionHandlerMetadata(bool isSingleton)
{
    public bool IsSingleton => isSingleton;
}

internal sealed class ClassHttpInteractionHandlerMetadata(Type handlerType, bool isSingleton, Func<IServiceProvider, object> instanceFactory) : HttpInteractionHandlerMetadata(isSingleton)
{
    public Type HandlerType => handlerType;

    public Func<IServiceProvider, object> InstanceFactory => instanceFactory;
}

internal sealed class DelegateHttpInteractionHandlerMetadata(Func<Interaction, IServiceProvider, ValueTask> handler, bool isSingleton) : HttpInteractionHandlerMetadata(isSingleton)
{
    public Func<Interaction, IServiceProvider, ValueTask> Handler => handler;
}
