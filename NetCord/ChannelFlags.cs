namespace NetCord;

[Flags]
public enum ChannelFlags
{
    Pinned = 1 << 1,
    RequireTag = 1 << 4,
}
