using System.Diagnostics;

namespace NetCord.Gateway;

internal class LatencyTimer
{
    private readonly Stopwatch _stopwatch = new();

    public void Start()
    {
        _stopwatch.Restart();
    }

    public TimeSpan Elapsed => _stopwatch.Elapsed;
}
