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

// internal sealed class HttpInteractionProcessor(
//     IServiceProvider services) : HttpEventProcessor<IInteraction>(services), IHttpInteractionProcessor
// {
//     private readonly ILogger<HttpInteractionProcessor> _logger = services.GetRequiredService<ILogger<HttpInteractionProcessor>>();
//
//     private readonly Func<Interaction, ValueTask>[] _handlers = [.. services.GetServices<HttpInteractionHandlerMetadata>()
//                                                                             .Select(m => CreateInvokeDelegate(m, services))];
//
//     protected override IInteraction GetData(HttpContext context, ReadOnlySpan<byte> body)
//     {
//         var response = context.Response;
//         return HttpInteractionFactory.Create(body, async (interaction, interactionCallback, withResponse, properties, cancellationToken) =>
//         {
//             using var content = interactionCallback.Serialize();
//             response.ContentType = content.Headers.ContentType!.ToString();
//             await content.CopyToAsync(response.Body, cancellationToken).ConfigureAwait(false);
//             await response.CompleteAsync().ConfigureAwait(false);
//             return null;
//         }, _client);
//     }
//
//     protected override ValueTask HandleAsync(HttpContext context, IInteraction data)
//     {
//         return data switch
//         {
//             Interaction interaction => InvokeHandlersAsync(_handlers, interaction),
//             PingInteraction pingInteraction => new(pingInteraction.SendResponseAsync(InteractionCallback.Pong)),
//             _ => default,
//         };
//     }
//
//     protected override void LogHandlerException(Exception ex)
//     {
//         _logger.LogError(ex, "An error occurred while invoking an HTTP interaction handler.");
//     }
// }
