namespace NetCord.WebSockets;

public class WebSocketException : Exception
{
    internal WebSocketException(string? message) : base(message)
    {
    }
}