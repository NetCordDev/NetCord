namespace NetCord;

public class GuildInvite
{
    private readonly JsonModels.JsonGuildInvite _jsonEntity;

    public string Code => _jsonEntity.Code;

    public RestGuild? Guild { get; }

    public Channel? Channel { get; }

    public User? Inviter { get; }

    public GuildInviteTargetType? TargetType => _jsonEntity.TargetType;

    public User? TargetUser { get; }

    public Application? TargetApplication { get; }

    public int? ApproximatePresenceCount => _jsonEntity.ApproximatePresenceCount;

    public int? ApproximateMemberCount => _jsonEntity.ApproximateMemberCount;

    public DateTimeOffset? ExpiresAt => _jsonEntity.ExpiresAt;

    public StageInstance? StageInstance { get; }

    public GuildScheduledEvent? GuildScheduledEvent { get; }

    public GuildInviteMetadata? Metadata { get; }

    internal GuildInvite(JsonModels.JsonGuildInvite jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (_jsonEntity.Guild != null)
            Guild = new(_jsonEntity.Guild, client);
        if (_jsonEntity.Channel != null)
            Channel = Channel.CreateFromJson(_jsonEntity.Channel, client);
        if (_jsonEntity.Inviter != null)
            Inviter = new(_jsonEntity.Inviter, client);
        if (_jsonEntity.TargetUser != null)
            TargetUser = new(_jsonEntity.TargetUser, client);
        if (_jsonEntity.TargetApplication != null)
            TargetApplication = new(_jsonEntity.TargetApplication, client);
        if (_jsonEntity.StageInstance != null)
            StageInstance = new(_jsonEntity.StageInstance, client);
        if (_jsonEntity.GuildScheduledEvent != null)
            GuildScheduledEvent = new(_jsonEntity.GuildScheduledEvent, client);
        if (_jsonEntity.Metadata != null)
            Metadata = new(_jsonEntity.Metadata);
    }
}

public enum GuildInviteTargetType
{
    Stream = 1,
    EmbeddedApplication = 2,
}