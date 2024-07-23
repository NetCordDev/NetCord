namespace NetCord.Rest;

public partial class RestInvite : IInvite, IJsonModel<JsonModels.JsonRestInvite>
{
    JsonModels.JsonRestInvite IJsonModel<JsonModels.JsonRestInvite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonRestInvite _jsonModel;

    private readonly RestClient _client;

    public InviteType Type => _jsonModel.Type;

    public string Code => _jsonModel.Code;

    public RestGuild? Guild { get; }

    public Channel? Channel { get; }

    public User? Inviter { get; }

    public InviteTargetType? TargetType => _jsonModel.TargetType;

    public User? TargetUser { get; }

    public Application? TargetApplication { get; }

    public int? ApproximatePresenceCount => _jsonModel.ApproximatePresenceCount;

    public int? ApproximateUserCount => _jsonModel.ApproximateUserCount;

    public DateTimeOffset? ExpiresAt => _jsonModel.ExpiresAt;

    public StageInstance? StageInstance { get; }

    public GuildScheduledEvent? GuildScheduledEvent { get; }

    public int? Uses => _jsonModel.Uses;

    public int? MaxUses => _jsonModel.MaxUses;

    public int? MaxAge => _jsonModel.MaxAge;

    public bool? Temporary => _jsonModel.Temporary;

    public DateTimeOffset? CreatedAt => _jsonModel.CreatedAt;

    ulong? IInvite.GuildId => Guild?.Id;

    ulong? IInvite.ChannelId => Channel?.Id;

    public RestInvite(JsonModels.JsonRestInvite jsonModel, RestClient client)
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

        _client = client;
    }
}
