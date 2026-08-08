using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

    public WebhookEventHandlerInvoker(ILogger<WebhookEventHandlerInvoker> logger, IEnumerable<WebhookHandlerMetadata> handlersMetadata, IServiceProvider services)
    {
        StorageBuilder builder = new();

        foreach (var handlerMetadata in handlersMetadata)
        {
            if (handlerMetadata is ClassWebhookHandlerMetadata classHandlerMetadata)
                builder.RegisterClassHandler(classHandlerMetadata, services);
            else
                builder.RegisterDelegateHandler((DelegateWebhookHandlerMetadata)handlerMetadata, services);
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

public interface IWebhookEventProcessor
{
    /// <summary>
    /// Processes an incoming webhook event request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the incoming request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public ValueTask ProcessAsync(HttpContext context);
}

internal sealed class WebhookEventProcessor(IServiceProvider services) : IWebhookEventProcessor
{
    private readonly IWebhookEventParser _parser = services.GetService<IWebhookEventParser>()
            ?? new WebhookEventParser(services.GetRequiredService<RestClient>(), services.GetRequiredService<IOptions<IDiscordOptions>>());

    private readonly IWebhookEventHandlerInvoker _invoker = services.GetService<IWebhookEventHandlerInvoker>()
            ?? new WebhookEventHandlerInvoker(services.GetRequiredService<ILogger<WebhookEventHandlerInvoker>>(), services.GetServices<WebhookHandlerMetadata>(), services);

    public async ValueTask ProcessAsync(HttpContext context)
    {
        switch (await _parser.ParseAsync(context).ConfigureAwait(false))
        {
            case null:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                break;
            case WebhookEventArgs args:
                await _invoker.InvokeAsync(args).ConfigureAwait(false);
                break;
            case PingWebhookEventArgs:
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
        }
    }
}

// [GenerateHandler("APPLICATION_AUTHORIZED", typeof(ApplicationAuthorizedWebhookEventArgs))]
// [GenerateHandler("APPLICATION_DEAUTHORIZED", typeof(ApplicationDeauthorizedWebhookEventArgs))]
// [GenerateHandler("ENTITLEMENT_CREATE", typeof(EntitlementCreateWebhookEventArgs))]
// [GenerateHandler(null, typeof(UnknownEventWebhookEventArgs))]
// internal partial class WebhookEventProcessor : HttpEventProcessor<JsonWebhookEventArgs>, IWebhookEventProcessor
// {
//     private partial class StorageBuilder;
//
//     private partial class Storage;
//
//     public WebhookEventProcessor(IServiceProvider services) : base(services)
//     {
//         StorageBuilder builder = new();
//
//         foreach (var handler in services.GetServices<WebhookHandlerMetadata>())
//         {
//             if (handler is ClassWebhookHandlerMetadata classHandlerMetadata)
//                 builder.RegisterClassHandler(classHandlerMetadata, services);
//             else
//                 builder.RegisterDelegateHandler((DelegateWebhookHandlerMetadata)handler, services);
//         }
//
//         _storage = builder.Build();
//
//         _logger = services.GetRequiredService<ILogger<WebhookEventProcessor>>();
//     }
//
//     private readonly Storage _storage;
//
//     private readonly ILogger<WebhookEventProcessor> _logger;
//
//     protected override JsonWebhookEventArgs GetData(HttpContext context, ReadOnlySpan<byte> body)
//     {
//         return WebhookEventArgsFactory.CreateJson(body);
//     }
//
//     protected override ValueTask HandleAsync(HttpContext context, JsonWebhookEventArgs data)
//     {
//         switch (data.Type)
//         {
//             case WebhookEventType.Event:
//                 return HandleEventAsync(data);
//             case WebhookEventType.Ping:
//                 context.Response.StatusCode = StatusCodes.Status204NoContent;
//                 break;
//         }
//
//         return default;
//     }
//
//     protected override void LogHandlerException(Exception ex)
//     {
//         _logger.LogError(ex, "An error occurred while invoking a webhook event handler.");
//     }
// }
