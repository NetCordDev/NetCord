namespace NetCord.Rest;

public class RestGuildInvite : IJsonModel<JsonModels.JsonRestGuildInvite>
{
    JsonModels.JsonRestGuildInvite IJsonModel<JsonModels.JsonRestGuildInvite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonRestGuildInvite _jsonModel;

    private readonly RestClient _client;

    public string Code => _jsonModel.Code;

    public RestGuild? Guild { get; }

    public Channel? Channel { get; }

    public User? Inviter { get; }

    public GuildInviteTargetType? TargetType => _jsonModel.TargetType;

    public User? TargetUser { get; }

    public Application? TargetApplication { get; }

    public int? ApproximatePresenceCount => _jsonModel.ApproximatePresenceCount;

    public int? ApproximateUserCount => _jsonModel.ApproximateUserCount;

    public DateTimeOffset? ExpiresAt => _jsonModel.ExpiresAt;

    public StageInstance? StageInstance { get; }

    public GuildScheduledEvent? GuildScheduledEvent { get; }

    public GuildInviteMetadata? Metadata { get; }

    public RestGuildInvite(JsonModels.JsonRestGuildInvite jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        var guild = jsonModel.Guild;
        if (guild is not null)
            Guild = new(guild, client);

        var channel = jsonModel.Channel;
        if (channel is not null)
            Channel = Channel.CreateFromJson(channel, client);

        var inviter = jsonModel.Inviter;
        if (inviter is not null)
            Inviter = new(inviter, client);

        var targetUser = jsonModel.TargetUser;
        if (targetUser is not null)
            TargetUser = new(targetUser, client);

        var targetApplication = jsonModel.TargetApplication;
        if (targetApplication is not null)
            TargetApplication = new(targetApplication, client);

        var stageInstance = jsonModel.StageInstance;
        if (stageInstance is not null)
            StageInstance = new(stageInstance, client);

        var guildScheduledEvent = jsonModel.GuildScheduledEvent;
        if (guildScheduledEvent is not null)
            GuildScheduledEvent = new(guildScheduledEvent, client);

        var metadata = jsonModel.Metadata;
        if (metadata is not null)
            Metadata = new(metadata);

        _client = client;
    }

    #region Invite
    public Task<RestGuildInvite> DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildInviteAsync(Code, properties);
    #endregion
}
