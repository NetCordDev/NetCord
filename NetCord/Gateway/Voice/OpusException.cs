namespace NetCord.Gateway.Voice;

public class OpusException : Exception
{
    public OpusError OpusError { get; }

    public OpusException(OpusError error) : base($"Opus returned an '{error}' error.")
    {
        OpusError = error;
    }
}
