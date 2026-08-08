namespace NetCord.Hosting.AspNetCore;

public interface IWebhookHandler;

internal abstract class WebhookHandlerMetadata(bool isSingleton)
{
    public bool IsSingleton => isSingleton;
}

internal sealed class ClassWebhookHandlerMetadata(Type handlerType, bool isSingleton, Func<IServiceProvider, object> instanceFactory) : WebhookHandlerMetadata(isSingleton)
{
    public Type HandlerType => handlerType;

    public Func<IServiceProvider, object> InstanceFactory => instanceFactory;
}

internal sealed class DelegateWebhookHandlerMetadata(Delegate handler, WebhookEventId eventId, bool isSingleton) : WebhookHandlerMetadata(isSingleton)
{
    public Delegate Handler => handler;

    internal WebhookEventId EventId => eventId;
}
