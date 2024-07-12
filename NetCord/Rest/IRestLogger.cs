using System.Text;

using NetCord.Logging;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public interface IRestLogger : IDisposable
{
    public void Log(LogLevel logLevel, Exception? exception, string message, params object?[] args);
}
