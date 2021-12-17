namespace NetCord.WebSockets
{
    internal class WebSocketException : Exception
    {
        public WebSocketException(string? message) : base(message)
        {
        }
    }
}
