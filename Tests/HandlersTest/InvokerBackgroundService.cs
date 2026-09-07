using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandlersTest;

internal class InvokerBackgroundService<TInvoker>(IServiceProvider services, Func<TInvoker, IServiceProvider, CancellationToken, ValueTask> invokeAction) : BackgroundService where TInvoker : notnull
{
    private readonly TInvoker _invoker = services.GetRequiredService<TInvoker>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await invokeAction(_invoker, services, stoppingToken).ConfigureAwait(false);

                await Task.Delay(Helper.DelayMilliseconds, stoppingToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
