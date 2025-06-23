using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetCord.Rest;
using NetCord.Rest.JsonModels;

namespace NetCord.Hosting.AspNetCore;

[GenerateHandler("APPLICATION_AUTHORIZED", typeof(ApplicationAuthorizedWebhookEventArgs))]
[GenerateHandler("APPLICATION_DEAUTHORIZED", typeof(ApplicationDeauthorizedWebhookEventArgs))]
[GenerateHandler("ENTITLEMENT_CREATE", typeof(EntitlementCreateWebhookEventArgs))]
[GenerateHandler(null, typeof(UnknownEventWebhookEventArgs))]
internal partial class WebhookEventHandler : HttpEventHandler<JsonWebhookEventArgs>
{
    private partial class StorageBuilder;

    private partial class Storage;

    public WebhookEventHandler(IServiceProvider services, string pattern) : base(services, pattern)
    {
        StorageBuilder builder = new();

        foreach (var handler in services.GetServices<IWebhookHandler>())
        {
            if (handler is IDelegateWebhookHandlerBase delegateHandler)
                builder.RegisterDelegateHandler(delegateHandler);
            else
                builder.RegisterClassHandler(handler);
        }

        _storage = builder.Build();

        _logger = services.GetRequiredService<ILogger<WebhookEventHandler>>();
    }

    private readonly Storage _storage;

    private readonly ILogger<WebhookEventHandler> _logger;

    protected override JsonWebhookEventArgs GetData(HttpContext context, ReadOnlySpan<byte> body)
    {
        return WebhookEventArgsFactory.CreateJson(body);
    }

    protected override ValueTask HandleAsync(HttpContext context, JsonWebhookEventArgs data)
    {
        switch (data.Type)
        {
            case WebhookEventType.Event:
                return HandleEventAsync(data);
            case WebhookEventType.Ping:
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                break;
        }

        return default;
    }

    protected override void LogHandlerException(Exception ex)
    {
        _logger.LogError(ex, "An error occurred while invoking a webhook event handler.");
    }
}
