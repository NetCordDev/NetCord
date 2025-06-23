namespace NetCord.Hosting.AspNetCore;

public interface IWebhookEventHandler;

internal interface IDelegateWebhookEventHandlerBase : IWebhookEventHandler
{
    internal string? RawName { get; }
}

internal interface IDelegateWebhookEventHandler<T> : IDelegateWebhookEventHandlerBase
{
    public ValueTask HandleAsync(T arg);
}

internal class DelegateWebhookEventHandler<T>(string? rawName, IServiceProvider services, Delegate handler) : IDelegateWebhookEventHandler<T>
{
    private readonly Func<T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<T, IServiceProvider, ValueTask>>(handler, [typeof(T)]);

    string? IDelegateWebhookEventHandlerBase.RawName => rawName;

    public ValueTask HandleAsync(T arg) => _handler(arg, services);
}
