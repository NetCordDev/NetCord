namespace NetCord.Rest.RateLimits;

internal class AdjustableSemaphoreSlim
{
    private readonly object _lock = new();
    private readonly LinkedList<TaskCompletionSource> _sources = new();
    private int _maxCount;
    private int _available;

    public int MaxCount
    {
        get => _maxCount;
        set
        {
            lock (_lock)
            {
                int diff = value - _maxCount;
                for (int i = Math.Min(diff, _sources.Count); i > 0; i--)
                {
                    _sources.First!.Value.SetResult();
                    _sources.RemoveFirst();
                }
                _available += diff;
                _maxCount = value;
            }
        }
    }

    public AdjustableSemaphoreSlim(int maxCount)
    {
        _available = _maxCount = maxCount;
    }

    public void Release()
    {
        lock (_lock)
        {
            if (_available++ >= -_sources.Count && _sources.Count != 0)
            {
                _sources.First!.Value.SetResult();
                _sources.RemoveFirst();
            }
        }
    }

    public Task WaitAsync()
    {
        lock (_lock)
        {
            if (_available-- > 0)
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
