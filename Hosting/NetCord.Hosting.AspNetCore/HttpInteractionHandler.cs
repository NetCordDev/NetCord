using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

internal sealed class HttpInteractionHandler(IServiceProvider services,
                                             string pattern) : HttpEventHandler<IInteraction>(services,
                                                                                              pattern)
{
    private readonly ILogger<HttpInteractionHandler> _logger = services.GetRequiredService<ILogger<HttpInteractionHandler>>();

    private readonly Func<Interaction, ValueTask>[] _handlers = [.. services.GetServices<IHttpInteractionHandler>()
                                                                            .Select<IHttpInteractionHandler, Func<Interaction, ValueTask>>(h => h.HandleAsync)];

    protected override IInteraction GetData(HttpContext context, ReadOnlySpan<byte> body)
    {
        var response = context.Response;
        return HttpInteractionFactory.Create(body, async (interaction, interactionCallback, withResponse, properties, cancellationToken) =>
        {
            using var content = interactionCallback.Serialize();
            response.ContentType = content.Headers.ContentType!.ToString();
            await content.CopyToAsync(response.Body, cancellationToken).ConfigureAwait(false);
            await response.CompleteAsync().ConfigureAwait(false);
            return null;
        }, _client);
    }

    protected override ValueTask HandleAsync(HttpContext context, IInteraction data)
    {
        return data switch
        {
            Interaction interaction => InvokeHandlersAsync(_handlers, interaction),
            PingInteraction pingInteraction => new(pingInteraction.SendResponseAsync(InteractionCallback.Pong)),
            _ => default,
        };
    }

    protected override void LogHandlerException(Exception ex)
    {
        _logger.LogError(ex, "An error occurred while invoking an HTTP interaction handler.");
    }
}
