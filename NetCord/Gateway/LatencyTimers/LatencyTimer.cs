using System.Diagnostics;

namespace NetCord.Gateway;

public class LatencyTimer : ILatencyTimer
{
    private readonly Stopwatch _stopwatch = new();

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public void Start()
    {
        _stopwatch.Restart();
    }
}
