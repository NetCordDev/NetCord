namespace NetCord;

public static class SnowflakeUtils
{
    public static ulong Create(DateTimeOffset createdAt)
    {
        return (ulong)((createdAt.ToUnixTimeMilliseconds() - Discord.Epoch) << 22);
    }

    public static ulong Create(DateTimeOffset createdAt, byte internalWorkerId, byte internalProcessId, ushort increment)
    {
        var c = (ulong)((createdAt.ToUnixTimeMilliseconds() - Discord.Epoch) << 22);
        var w = (ulong)((internalWorkerId << 17) & 0x3E0000);
        var p = (ulong)((internalProcessId << 12) & 0x1F000);
        var i = (ulong)(increment & 0xFFF);
        return c | w | p | i;
    }

    public static DateTimeOffset CreatedAt(ulong id) => DateTimeOffset.FromUnixTimeMilliseconds((long)((id >> 22) + Discord.Epoch));

    public static byte InternalWorkerId(ulong id) => (byte)((id & 0x3E0000) >> 17);

    public static byte InternalProcessId(ulong id) => (byte)((id & 0x1F000) >> 12);

    public static ushort Increment(ulong id) => (ushort)(id & 0xFFF);
}
