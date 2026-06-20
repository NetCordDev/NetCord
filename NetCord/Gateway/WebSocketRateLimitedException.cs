
namespace NetCord.Gateway;

public class WebSocketRateLimitedException(int resetAfter) : Exception($"Rate limit exceeded. Reset after {resetAfter} ms.")
{
    /// <summary>
    /// The time in milliseconds after which a message can be sent again.
    /// </summary>
    public int ResetAfter => resetAfter;
}
