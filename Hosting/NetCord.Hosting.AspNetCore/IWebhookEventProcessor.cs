using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

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
            ?? new WebhookEventHandlerInvoker(services.GetRequiredService<ILogger<WebhookEventHandlerInvoker>>(), services.GetServices<IWebhookHandlerMetadata>(), services);

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
