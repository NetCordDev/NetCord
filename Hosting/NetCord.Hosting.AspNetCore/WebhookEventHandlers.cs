namespace NetCord.Hosting.AspNetCore;

public interface IWebhookEventHandlerBase
{
}

public interface IWebhookEventHandler<T> : IWebhookEventHandlerBase
{
    /// <summary>
    /// Handles the webhook event.
    /// </summary>
    /// <param name="arg">The event argument.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask HandleAsync(T arg);
}

internal class DelegateWebhookEventHandler<T>(IServiceProvider services, Delegate handler) : IWebhookEventHandler<T>
{
    private readonly Func<T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<T, IServiceProvider, ValueTask>>(handler, [typeof(T)]);

    public ValueTask HandleAsync(T arg) => _handler(arg, services);
}
