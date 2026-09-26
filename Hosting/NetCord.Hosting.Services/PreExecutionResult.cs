namespace NetCord.Hosting.Services;

public abstract class PreExecutionResult
{
    public static PreExecutionResult Continue => ContinuePreExecutionResult.Instance;

    public static PreExecutionResult Skip => SkipPreExecutionResult.Instance;

    private protected PreExecutionResult()
    {
    }
}

public class ContinuePreExecutionResult : PreExecutionResult
{
    internal static ContinuePreExecutionResult Instance { get; } = new();

    private ContinuePreExecutionResult()
    {
    }
}

public class SkipPreExecutionResult : PreExecutionResult
{
    internal static SkipPreExecutionResult Instance { get; } = new();

    private SkipPreExecutionResult()
    {
    }
}
