using System.Diagnostics.CodeAnalysis;

namespace NetCord.Gateway;

internal class ShardedGatewayClientEventManager
{
    private readonly Dictionary<(GatewayClient, Lock), Delegate> _events = [];

    public bool AddEvent(GatewayClient client, Lock @lock, Func<ValueTask> @delegate)
    {
        return _events.TryAdd((client, @lock), @delegate);
    }

    public bool AddEvent<T>(GatewayClient client, Lock @lock, Func<T, ValueTask> @delegate)
    {
        return _events.TryAdd((client, @lock), @delegate);
    }

    public bool RemoveEvent(GatewayClient client, Lock @lock, [MaybeNullWhen(false)] out Func<ValueTask> @delegate)
    {
        if (_events.Remove((client, @lock), out var d))
        {
            @delegate = (Func<ValueTask>)d;
            return true;
        }

        @delegate = null;
        return false;
    }

    public bool RemoveEvent<T>(GatewayClient client, Lock @lock, [MaybeNullWhen(false)] out Func<T, ValueTask> @delegate)
    {
        if (_events.Remove((client, @lock), out var d))
        {
            @delegate = (Func<T, ValueTask>)d;
            return true;
        }

        @delegate = null;
        return false;
    }
}
