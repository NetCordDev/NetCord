namespace NetCord.Services;

public class NotFoundResult : IFailResult
{
    internal static NotFoundResult Command { get; } = new("Command not found.");
    internal static NotFoundResult ComponentInteraction { get; } = new("Component interaction not found.");

    private NotFoundResult(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
