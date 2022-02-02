namespace NetCord.Services.Commands;

public class ParameterCountException : Exception
{
    internal ParameterCountException(string? message) : base(message)
    {
    }
}