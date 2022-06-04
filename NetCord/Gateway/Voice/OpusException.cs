namespace NetCord.Gateway.Voice;

public class OpusException : Exception
{
    public OpusError OpusError { get; }

    internal OpusException(OpusError error) : base(error.ToString())
    {
        OpusError = error;
    }
}