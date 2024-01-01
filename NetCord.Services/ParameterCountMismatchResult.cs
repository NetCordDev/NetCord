using System.ComponentModel;

namespace NetCord.Services;

public class ParameterCountMismatchResult : IFailResult
{
    public ParameterCountMismatchResult(ParameterCountMismatchType type)
    {
        Type = type;
    }

    public ParameterCountMismatchType Type { get; }

    public string Message => Type switch
    {
        ParameterCountMismatchType.TooFew => "Too few parameters.",
        ParameterCountMismatchType.TooMany => "Too many parameters.",
        _ => throw new InvalidEnumArgumentException(nameof(Type), (int)Type, typeof(ParameterCountMismatchType)),
    };
}
