using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NetCord.Gateway;

internal static class WebSocketClientThrowHelper
{
    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowConnectionAlreadyStarted()
    {
        throw new InvalidOperationException("Connection already started.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowConnectionNotStarted(Exception? innerException = null)
    {
        throw new InvalidOperationException("Connection not started.", innerException);
    }
}
