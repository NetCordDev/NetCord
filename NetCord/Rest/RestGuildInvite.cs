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

    public int? ApproximateMemberCount => _jsonModel.ApproximateMemberCount;

    public DateTimeOffset? ExpiresAt => _jsonModel.ExpiresAt;

    public StageInstance? StageInstance { get; }

    public GuildScheduledEvent? GuildScheduledEvent { get; }

    public GuildInviteMetadata? Metadata { get; }

    public RestGuildInvite(JsonModels.JsonRestGuildInvite jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.Guild != null)
            Guild = new(_jsonModel.Guild, client);
        if (_jsonModel.Channel != null)
            Channel = Channel.CreateFromJson(_jsonModel.Channel, client);
        if (_jsonModel.Inviter != null)
            Inviter = new(_jsonModel.Inviter, client);
        if (_jsonModel.TargetUser != null)
            TargetUser = new(_jsonModel.TargetUser, client);
        if (_jsonModel.TargetApplication != null)
            TargetApplication = new(_jsonModel.TargetApplication, client);
        if (_jsonModel.StageInstance != null)
            StageInstance = new(_jsonModel.StageInstance, client);
        if (_jsonModel.GuildScheduledEvent != null)
            GuildScheduledEvent = new(_jsonModel.GuildScheduledEvent, client);
        if (_jsonModel.Metadata != null)
            Metadata = new(_jsonModel.Metadata);
        _client = client;
    }

    #region Invite
    public Task<RestGuildInvite> DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildInviteAsync(Code, properties);
    #endregion
}