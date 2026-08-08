using System.Buffers;
using System.Runtime.CompilerServices;

namespace NetCord.Hosting.AspNetCore;

internal abstract class HttpEventHandlerInvoker
{
    protected abstract void LogHandlerException(Exception ex);

    protected ValueTask InvokeHandlersAsync(Func<ValueTask>[] handlers)
    {
        int length = handlers.Length;

        if (length is 0)
            return default;

        var tasks = ArrayPool<ValueTask>.Shared.Rent(length);

        for (int i = 0; i < length; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i]();
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogHandlerException(ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(length, tasks);
    }

    protected ValueTask InvokeHandlersAsync<THandlerData>(Func<THandlerData, ValueTask>[] handlers, THandlerData data)
    {
        int length = handlers.Length;

        if (length is 0)
            return default;

        var tasks = ArrayPool<ValueTask>.Shared.Rent(length);

        for (int i = 0; i < length; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogHandlerException(ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(length, tasks);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask HandleTasksAsync(int length, ValueTask[] tasks)
    {
        for (int i = 0; i < length; i++)
        {
            try
            {
                await tasks[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogHandlerException(ex);
            }
        }

        ArrayPool<ValueTask>.Shared.Return(tasks);
    }
}

