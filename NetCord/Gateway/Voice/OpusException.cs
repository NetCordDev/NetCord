namespace NetCord.Gateway.Voice;

public class OpusException(OpusError error) : Exception($"Opus returned an '{error}' error.")
{
    public OpusError OpusError { get; } = error;
}
