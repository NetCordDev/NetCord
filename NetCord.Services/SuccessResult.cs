namespace NetCord.Services;

internal class SuccessResult : IExecutionResult
{
    internal static SuccessResult Instance { get; } = new();
}
