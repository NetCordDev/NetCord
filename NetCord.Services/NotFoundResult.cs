namespace NetCord.Services;

public class NotFoundResult : IFailResult
{
    public NotFoundResult(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
