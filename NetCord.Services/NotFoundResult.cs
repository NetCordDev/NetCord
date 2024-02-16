namespace NetCord.Services;

public class NotFoundResult(string message) : IFailResult
{
    public string Message { get; } = message;
}
