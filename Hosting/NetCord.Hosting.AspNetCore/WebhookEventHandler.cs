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
        List<IWebhookEventHandler<ApplicationAuthorizedWebhookEventArgs>> applicationAuthorizedHandlers = [];
        List<IWebhookEventHandler<ApplicationDeauthorizedWebhookEventArgs>> applicationDeauthorizedHandlers = [];
        List<IWebhookEventHandler<EntitlementCreateWebhookEventArgs>> entitlementCreateHandlers = [];
        List<IWebhookEventHandler<UnknownEventWebhookEventArgs>> unknownHandlers = [];

        foreach (var handler in services.GetServices<IWebhookEventHandlerBase>())
        {
            if (handler is IWebhookEventHandler<ApplicationAuthorizedWebhookEventArgs> applicationAuthorizedHandler)
                applicationAuthorizedHandlers.Add(applicationAuthorizedHandler);

            if (handler is IWebhookEventHandler<ApplicationDeauthorizedWebhookEventArgs> applicationDeauthorizedHandler)
                applicationDeauthorizedHandlers.Add(applicationDeauthorizedHandler);

            if (handler is IWebhookEventHandler<EntitlementCreateWebhookEventArgs> entitlementCreateHandler)
                entitlementCreateHandlers.Add(entitlementCreateHandler);

            if (handler is IWebhookEventHandler<UnknownEventWebhookEventArgs> unknownHandler)
                unknownHandlers.Add(unknownHandler);
        }

        _applicationAuthorized = [.. applicationAuthorizedHandlers];
        _applicationDeauthorized = [.. applicationDeauthorizedHandlers];
        _entitlementCreate = [.. entitlementCreateHandlers];
        _unknownEvents = [.. unknownHandlers];

        _logger = services.GetRequiredService<ILogger<WebhookEventHandler>>();
    }

    private readonly IWebhookEventHandler<ApplicationAuthorizedWebhookEventArgs>[] _applicationAuthorized;

    private readonly IWebhookEventHandler<ApplicationDeauthorizedWebhookEventArgs>[] _applicationDeauthorized;

    private readonly IWebhookEventHandler<EntitlementCreateWebhookEventArgs>[] _entitlementCreate;

    private readonly IWebhookEventHandler<UnknownEventWebhookEventArgs>[] _unknownEvents;

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
            _ => InvokeHandlersAsync(_unknownEvents, () => new UnknownEventWebhookEventArgs(data)),
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
