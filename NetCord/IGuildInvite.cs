namespace NetCord;

public interface IGuildInvite
{
    public ulong? GuildId { get; }
    public ulong? ChannelId { get; }
    public string Code { get; }
    public User? Inviter { get; }
    public User? TargetUser { get; }
    public Application? TargetApplication { get; }
    public int? MaxAge { get; }
    public int? MaxUses { get; }
    public GuildInviteTargetType? TargetType { get; }
    public bool? Temporary { get; }
    public int? Uses { get; }
    public DateTimeOffset? CreatedAt { get; }
}
