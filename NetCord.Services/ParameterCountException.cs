using System.ComponentModel;

namespace NetCord.Services;

public class ParameterCountException : Exception
{
    public ParameterCountExceptionType Type { get; }

    public ParameterCountException(ParameterCountExceptionType type) : base(type switch
    {
        ParameterCountExceptionType.TooFew => "Too few parameters.",
        ParameterCountExceptionType.TooMany => "Too many parameters.",
        _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ParameterCountExceptionType)),
    })
    {
        Type = type;
    }
}
