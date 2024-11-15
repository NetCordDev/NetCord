namespace NetCord.Gateway.Voice;

internal enum VoiceOpcode : byte
{
    Identify = 0,
    SelectProtocol = 1,
    Ready = 2,
    Heartbeat = 3,
    SessionDescription = 4,
    Speaking = 5,
    HeartbeatACK = 6,
    Resume = 7,
    Hello = 8,
    Resumed = 9,
    ClientConnect = 11,
    ClientDisconnect = 13,
}
