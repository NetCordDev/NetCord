using Microsoft.Extensions.Logging;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

public interface IWebhookEventHandlerInvoker
{
    /// <summary>
    /// Invokes the appropriate handler for a parsed webhook event.
    /// </summary>
    /// <param name="args">The parsed webhook event data to be handled.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public ValueTask InvokeAsync(WebhookEventArgs args);
}

[GenerateHandler("APPLICATION_AUTHORIZED", typeof(ApplicationAuthorizedWebhookEventArgs))]
[GenerateHandler("APPLICATION_DEAUTHORIZED", typeof(ApplicationDeauthorizedWebhookEventArgs))]
[GenerateHandler("ENTITLEMENT_CREATE", typeof(EntitlementCreateWebhookEventArgs))]
[GenerateHandler(null, typeof(UnknownEventWebhookEventArgs))]
internal sealed partial class WebhookEventHandlerInvoker : HttpEventHandlerInvoker, IWebhookEventHandlerInvoker
{
    private readonly ILogger<WebhookEventHandlerInvoker> _logger;

    private readonly Storage _storage;

    public WebhookEventHandlerInvoker(ILogger<WebhookEventHandlerInvoker> logger, IEnumerable<IWebhookHandlerMetadata> handlersMetadata, IServiceProvider services)
    {
        StorageBuilder builder = new();

        foreach (var handlerMetadata in handlersMetadata)
        {
            if (handlerMetadata is ClassHandlerMetadata classHandlerMetadata)
                builder.RegisterClassHandler(classHandlerMetadata, services);
            else
                builder.RegisterDelegateHandler((DelegateHandlerMetadata<WebhookEventId>)handlerMetadata, services);
        }

        _logger = logger;
        _storage = builder.Build();
    }

    public ValueTask InvokeAsync(WebhookEventArgs args)
    {
        return HandleEventAsync(args);
    }

    protected override void LogHandlerException(Exception ex)
    {
        _logger.LogError(ex, "An error occurred while invoking a webhook event handler.");
    }
}
