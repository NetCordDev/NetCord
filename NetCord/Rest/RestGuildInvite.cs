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

    public IReadOnlyList<Role>? Roles { get; }

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

        if (jsonModel.Guild is { } guild)
        {
            Guild = new(guild, client);

            var guildId = Guild.Id;

            if (jsonModel.Roles is { } roles)
                Roles = roles.Select(role => new Role(role, guildId, client)).ToArray();
        }

        if (jsonModel.Channel is { } channel)
            Channel = Channel.CreateFromJson(channel, client);

        if (jsonModel.Inviter is { } inviter)
            Inviter = new(inviter, client);

        if (jsonModel.TargetUser is { } targetUser)
            TargetUser = new(targetUser, client);

        if (jsonModel.TargetApplication is { } targetApplication)
            TargetApplication = new(targetApplication, client);

        if (jsonModel.StageInstance is { } stageInstance)
            StageInstance = new(stageInstance, client);

        if (jsonModel.GuildScheduledEvent is { } guildScheduledEvent)
            GuildScheduledEvent = new(guildScheduledEvent, client);

        _client = client;
    }
}
