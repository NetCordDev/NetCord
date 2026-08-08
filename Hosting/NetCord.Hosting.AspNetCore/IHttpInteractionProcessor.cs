using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

public interface IHttpInteractionProcessor
{
    /// <summary>
    /// Processes an incoming HTTP interaction request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the incoming request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public ValueTask ProcessAsync(HttpContext context);
}

internal sealed class HttpInteractionProcessor(IServiceProvider services) : IHttpInteractionProcessor
{
    private readonly IHttpInteractionParser _parser = services.GetService<IHttpInteractionParser>()
            ?? new HttpInteractionParser(services.GetRequiredService<RestClient>(), services.GetRequiredService<IOptions<IDiscordOptions>>());

    private readonly IHttpInteractionHandlerInvoker _invoker = services.GetService<IHttpInteractionHandlerInvoker>()
            ?? new HttpInteractionHandlerInvoker(services.GetRequiredService<ILogger<HttpInteractionHandlerInvoker>>(), services.GetServices<HttpInteractionHandlerMetadata>(), services);

    public async ValueTask ProcessAsync(HttpContext context)
    {
        switch (await _parser.ParseAsync(context).ConfigureAwait(false))
        {
            case null:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            case Interaction interaction:
                await _invoker.InvokeAsync(interaction).ConfigureAwait(false);
                break;
            case PingInteraction pingInteraction:
                await pingInteraction.SendResponseAsync(InteractionCallback.Pong).ConfigureAwait(false);
                return;
            default:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
        }
    }
}
