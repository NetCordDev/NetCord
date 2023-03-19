namespace NetCord.Gateway.Voice;

public enum OpusError
{
    /// <summary>
    /// No error.
    /// </summary>
    NoError = 0,

    /// <summary>
    /// One or more invalid/out of range arguments.
    /// </summary>
    BadArg = -1,

    /// <summary>
    /// Not enough bytes allocated in the buffer.
    /// </summary>
    BufferToSmall = -2,

    /// <summary>
    /// An internal error was detected.
    /// </summary>
    InternalError = -3,

    /// <summary>
    /// The compressed data passed is corrupted.
    /// </summary>
    InvalidPacket = -4,

    /// <summary>
    /// Invalid/unsupported request number.
    /// </summary>
    Unimplemented = -5,

    /// <summary>
    /// An encoder or decoder structure is invalid or already freed.
    /// </summary>
    InvalidState = -6,

    /// <summary>
    /// Memory allocation has failed.
    /// </summary>
    AllocFail = -7,
}
