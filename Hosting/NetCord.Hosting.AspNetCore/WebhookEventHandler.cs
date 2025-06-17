using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetCord.Rest;
using NetCord.Rest.JsonModels;

namespace NetCord.Hosting.AspNetCore;

internal class WebhookEventHandler : HttpEventHandler<JsonWebhookEventArgs>
{
    public WebhookEventHandler(IServiceProvider services, string pattern) : base(services, pattern)
    {
        List<IWebhookEventHandler<ApplicationAuthorizedWebhookEventArgs>> applicationAuthorized = [];
        List<IWebhookEventHandler<ApplicationDeauthorizedWebhookEventArgs>> applicationDeauthorized = [];
        List<IWebhookEventHandler<EntitlementCreateWebhookEventArgs>> entitlementCreate = [];
        List<IWebhookEventHandler<UnknownEventWebhookEventArgs>> unknownEvent = [];

        foreach (var handler in services.GetServices<IWebhookEventHandlerBase>())
        {
            if (handler is IWebhookEventHandler<ApplicationAuthorizedWebhookEventArgs> applicationAuthorizedHandler)
                applicationAuthorized.Add(applicationAuthorizedHandler);

            if (handler is IWebhookEventHandler<ApplicationDeauthorizedWebhookEventArgs> applicationDeauthorizedHandler)
                applicationDeauthorized.Add(applicationDeauthorizedHandler);

            if (handler is IWebhookEventHandler<EntitlementCreateWebhookEventArgs> entitlementCreateHandler)
                entitlementCreate.Add(entitlementCreateHandler);

            if (handler is IWebhookEventHandler<UnknownEventWebhookEventArgs> unknownEventHandler)
                unknownEvent.Add(unknownEventHandler);
        }

        _applicationAuthorized = [.. applicationAuthorized];
        _applicationDeauthorized = [.. applicationDeauthorized];
        _entitlementCreate = [.. entitlementCreate];
        _unknownEvent = [.. unknownEvent];

        _logger = services.GetRequiredService<ILogger<WebhookEventHandler>>();
    }

    private readonly IWebhookEventHandler<ApplicationAuthorizedWebhookEventArgs>[] _applicationAuthorized;

    private readonly IWebhookEventHandler<ApplicationDeauthorizedWebhookEventArgs>[] _applicationDeauthorized;

    private readonly IWebhookEventHandler<EntitlementCreateWebhookEventArgs>[] _entitlementCreate;

    private readonly IWebhookEventHandler<UnknownEventWebhookEventArgs>[] _unknownEvent;

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

    private ValueTask HandleEventAsync(JsonWebhookEventArgs data)
    {
        return data.Event!.Type switch
        {
            "APPLICATION_AUTHORIZED" => InvokeHandlersAsync(_applicationAuthorized, () => new ApplicationAuthorizedWebhookEventArgs(data, _client)),
            "APPLICATION_DEAUTHORIZED" => InvokeHandlersAsync(_applicationDeauthorized, () => new ApplicationDeauthorizedWebhookEventArgs(data, _client)),
            "ENTITLEMENT_CREATE" => InvokeHandlersAsync(_entitlementCreate, () => new EntitlementCreateWebhookEventArgs(data, _client)),
            _ => InvokeHandlersAsync(_unknownEvent, () => new UnknownEventWebhookEventArgs(data)),
        };
    }

    protected override ValueTask InvokeHandlerAsync<THandler, THandlerData>(THandler handler, THandlerData data)
    {
        return Unsafe.As<THandler, IWebhookEventHandler<THandlerData>>(ref handler).HandleAsync(data);
    }

    protected override void LogHandlerException(Exception ex)
    {
        _logger.LogError(ex, "An error occurred while invoking a webhook event handler.");
    }
}
