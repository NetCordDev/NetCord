namespace NetCord.Hosting.AspNetCore;

public interface IWebhookHandler;

internal interface IDelegateWebhookHandlerBase : IWebhookHandler
{
    internal string? RawName { get; }
}

internal interface IDelegateWebhookHandler<T> : IDelegateWebhookHandlerBase
{
    public ValueTask HandleAsync(T arg);
}

internal class DelegateWebhookHandler<T>(string? rawName, IServiceProvider services, Delegate handler) : IDelegateWebhookHandler<T>
{
    private readonly Func<T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<T, IServiceProvider, ValueTask>>(handler, [typeof(T)]);

    string? IDelegateWebhookHandlerBase.RawName => rawName;

    public ValueTask HandleAsync(T arg) => _handler(arg, services);
}
