using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NetCord.Hosting.AspNetCore;

public interface IHttpInteractionHandlerInvoker
{
    /// <summary>
    /// Invokes the appropriate handler for a parsed HTTP interaction.
    /// </summary>
    /// <param name="interaction">The parsed interaction to be handled.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public ValueTask InvokeAsync(Interaction interaction);
}

internal sealed class HttpInteractionHandlerInvoker(ILogger<HttpInteractionHandlerInvoker> logger, IEnumerable<HttpInteractionHandlerMetadata> handlerMetadata, IServiceProvider services) : HttpEventHandlerInvoker, IHttpInteractionHandlerInvoker
{
    private readonly Func<Interaction, ValueTask>[] _handlers = [.. handlerMetadata.Select(m => CreateInvokeDelegate(m, services))];

    private static Func<Interaction, ValueTask> CreateInvokeDelegate(HttpInteractionHandlerMetadata handlerMetadata, IServiceProvider services)
    {
        return handlerMetadata is ClassHttpInteractionHandlerMetadata classHandlerMetadata
            ? CreateClassInvokeDelegate(classHandlerMetadata, services)
            : CreateDelegateInvokeDelegate((DelegateHttpInteractionHandlerMetadata)handlerMetadata, services);
    }

    private static Func<Interaction, ValueTask> CreateClassInvokeDelegate(ClassHttpInteractionHandlerMetadata handlerMetadata, IServiceProvider services)
    {
        var instanceFactory = handlerMetadata.InstanceFactory;

        return handlerMetadata.IsSingleton
            ? ((IHttpInteractionHandler)instanceFactory(services)).HandleAsync
            : async interaction =>
            {
                var scope = services.CreateAsyncScope();
                try
                {
                    await ((IHttpInteractionHandler)instanceFactory(scope.ServiceProvider)).HandleAsync(interaction).ConfigureAwait(false);
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

    public ValueTask InvokeAsync(Interaction data)
    {
        return InvokeHandlersAsync(_handlers, data);
    }

    protected override void LogHandlerException(Exception ex)
    {
        logger.LogError(ex, "An exception occurred while invoking an HTTP interaction handler.");
    }
}
