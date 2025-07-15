namespace NetCord.Services;

public class ParametersMismatchResult : IFailResult
{
    public static ParametersMismatchResult SlashCommandNotMatching { get; } = new("The actual parameters do not match the expected ones.");

    private protected ParametersMismatchResult(string message)
    {
        Message = message;
    }

    public string Message { get; }
}

public class ParameterCountMismatchResult : ParametersMismatchResult
{
    public static ParameterCountMismatchResult TooFew { get; } = new(ParameterCountMismatchType.TooFew, "Too few parameters provided.");

    public static ParameterCountMismatchResult TooMany { get; } = new(ParameterCountMismatchType.TooMany, "Too many parameters provided.");

    private ParameterCountMismatchResult(ParameterCountMismatchType type, string message) : base(message)
    {
        Type = type;
    }

    public ParameterCountMismatchType Type { get; }
}
