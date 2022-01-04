namespace NetCord;

public class GuildScheduledEvent : Entity
{
    private readonly JsonModels.JsonGuildScheduledEvent _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public DiscordId GuildId => _jsonEntity.GuildId;

    public DiscordId? ChannelId => _jsonEntity.ChannelId;

    public DiscordId? CreatorId => _jsonEntity.CreatorId;

    public string Name => _jsonEntity.Name;

    public string? Description => _jsonEntity.Description;

    public DateTimeOffset ScheduledStartTime => _jsonEntity.ScheduledStartTime;

    public DateTimeOffset? ScheduledEndTime => _jsonEntity.ScheduledEndTime;

    public GuildScheduledEventPrivacyLevel PrivacyLevel => _jsonEntity.PrivacyLevel;

    public GuildScheduledEventStatus Status => _jsonEntity.Status;

    public GuildScheduledEventEntityType EntityType => _jsonEntity.EntityType;

    public DiscordId? EntityId => _jsonEntity.EntityId;

    public string? Location => _jsonEntity.EntityMetadata?.Location;

    public User? Creator { get; }

    public int? UserCount => _jsonEntity.UserCount;

    internal GuildScheduledEvent(JsonModels.JsonGuildScheduledEvent jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (_jsonEntity.Creator != null)
            Creator = new(_jsonEntity.Creator, client);
    }
}