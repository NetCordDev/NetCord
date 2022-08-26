namespace NetCord.Gateway.Voice;

public enum OpusError
{
    NoError = 0,
    BadArg = -1,
    BufferToSmall = -2,
    InternalError = -3,
    InvalidPacket = -4,
    Unimplemented = -5,
    InvalidState = -6,
    AllocFail = -7,
}
