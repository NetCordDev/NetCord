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

internal sealed partial class HttpInteractionHandlerInvoker(ILogger<HttpInteractionHandlerInvoker> logger, IEnumerable<IHttpInteractionHandlerMetadata> handlerMetadata, IServiceProvider services) : HttpEventHandlerInvoker, IHttpInteractionHandlerInvoker
{
    private readonly Func<Interaction, ValueTask>[] _handlers = [.. handlerMetadata.Select(m => CreateInvokeDelegate(m, services))];

    private static Func<Interaction, ValueTask> CreateInvokeDelegate(IHttpInteractionHandlerMetadata handlerMetadata, IServiceProvider services)
    {
        return handlerMetadata is ClassHandlerMetadata classHandlerMetadata
            ? CreateClassInvokeDelegate(classHandlerMetadata, services)
            : CreateDelegateInvokeDelegate((DelegateHandlerMetadata)handlerMetadata, services);
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
