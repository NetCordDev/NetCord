using System.Diagnostics.CodeAnalysis;

namespace NetCord.Gateway;

internal static class GatewayClientThrowHelper
{
    [DoesNotReturn]
    public static void ThrowConnectionAlreadyStarted()
    {
        throw new InvalidOperationException("Connection already started.");
    }

    [DoesNotReturn]
    public static void ThrowConnectionNotStarted(Exception? innerException = null)
    {
        throw new InvalidOperationException("Connection not started.", innerException);
    }
}
