namespace NetCord.Hosting.Services;

internal static class ExecutionContextHelper
{
    public static void CaptureOrRestore(ref ExecutionContext? executionContext)
    {
        if (executionContext is null)
            executionContext = ExecutionContext.Capture();
        else
            ExecutionContext.Restore(executionContext);
    }
}
