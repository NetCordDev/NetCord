namespace NetCord.Rest.RateLimits;

internal class AdjustableSemaphoreSlim
{
    private readonly object _lock = new();
    private readonly LinkedList<TaskCompletionSource> _sources = new();
    private int _count;

    public AdjustableSemaphoreSlim(int initialCount)
    {
        _count = initialCount;
    }

    public void Release()
    {
        lock (_lock)
        {
            _count++;
            if (_sources.Count != 0)
            {
                _sources.First!.Value.SetResult();
                _sources.RemoveFirst();
            }
        }
    }

    public void Release(int count)
    {
        lock (_lock)
        {
            _count += count;
            for (int i = Math.Min(_sources.Count, count); i > 0; i--)
            {
                _sources.First!.Value.SetResult();
                _sources.RemoveFirst();
            }
        }
    }

    public void Enter(int count)
    {
        lock (_lock)
            _count -= count;
    }

    public Task WaitAsync()
    {
        lock (_lock)
        {
            if (--_count >= 0)
                return Task.CompletedTask;
            else
            {
                TaskCompletionSource source = new();
                _sources.AddLast(source);
                return source.Task;
            }
        }
    }
}
