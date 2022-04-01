namespace NetCord;

public class GuildInvite
{
    private readonly JsonModels.JsonGuildInvite _jsonEntity;

    internal GuildInvite(JsonModels.JsonGuildInvite jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Inviter != null)
            Inviter = new(jsonEntity.Inviter, client);
        if (jsonEntity.TargetUser != null)
            TargetUser = new(jsonEntity.TargetUser, client);
        if (jsonEntity.TargetApplication != null)
            TargetApplication = new(jsonEntity.TargetApplication, client);
    }

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public string Code => _jsonEntity.Code;

    public DateTimeOffset CreatedAt => _jsonEntity.CreatedAt;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public User? Inviter { get; }

    public int MaxAge => _jsonEntity.MaxAge;

    public int MaxUses => _jsonEntity.MaxUses;

    public GuildInviteTargetType? TargetType => _jsonEntity.TargetType;

    public User? TargetUser { get; }

    public Application? TargetApplication { get; }

    public bool Temporary => _jsonEntity.Temporary;

    public int Uses => _jsonEntity.Uses;
}
