using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

public interface IHttpInteractionProcessor
{
    /// <summary>
    /// Processes an incoming HTTP interaction request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the incoming request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ProcessAsync(HttpContext context);
}

internal sealed class HttpInteractionProcessor(
    IServiceProvider services) : HttpEventProcessor<IInteraction>(services), IHttpInteractionProcessor
{
    private readonly ILogger<HttpInteractionProcessor> _logger = services.GetRequiredService<ILogger<HttpInteractionProcessor>>();

    private readonly Func<Interaction, ValueTask>[] _handlers = [.. services.GetServices<HttpInteractionHandlerMetadata>()
                                                                            .Select(m => CreateInvokeDelegate(m, services))];

    private static Func<Interaction, ValueTask> CreateInvokeDelegate(HttpInteractionHandlerMetadata handlerMetadata, IServiceProvider services)
    {
        if (handlerMetadata is ClassHttpInteractionHandlerMetadata classHandlerMetadata)
            return CreateClassInvokeDelegate(classHandlerMetadata, services);
        else
            return CreateDelegateInvokeDelegate((DelegateHttpInteractionHandlerMetadata)handlerMetadata, services);
    }

    private static Func<Interaction, ValueTask> CreateClassInvokeDelegate(ClassHttpInteractionHandlerMetadata handlerMetadata, IServiceProvider services)
    {
        return handlerMetadata.IsSingleton
            ? ((IHttpInteractionHandler)handlerMetadata.InstanceFactory(services)).HandleAsync
            : async interaction =>
            {
                var scope = services.CreateAsyncScope();
                try
                {
                    await ((IHttpInteractionHandler)handlerMetadata.InstanceFactory(scope.ServiceProvider)).HandleAsync(interaction).ConfigureAwait(false);
                }
                finally
                {
                    await scope.DisposeAsync().ConfigureAwait(false);
                }
            };
    }

    private static Func<Interaction, ValueTask> CreateDelegateInvokeDelegate(DelegateHttpInteractionHandlerMetadata handlerMetadata, IServiceProvider services)
    {
        var handler = handlerMetadata.Handler;

        return handlerMetadata.IsSingleton
            ? interaction => handler(interaction, services)
            : async interaction =>
            {
                var scope = services.CreateAsyncScope();
                try
                {
                    await handler(interaction, scope.ServiceProvider).ConfigureAwait(false);
                }
                finally
                {
                    await scope.DisposeAsync().ConfigureAwait(false);
                }
            };
    }

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
