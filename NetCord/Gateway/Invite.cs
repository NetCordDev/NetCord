using NetCord.Rest;

namespace NetCord.Gateway;

public class Invite : IInvite, IJsonModel<JsonModels.JsonInvite>
{
    JsonModels.JsonInvite IJsonModel<JsonModels.JsonInvite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonInvite _jsonModel;

    public Invite(JsonModels.JsonInvite jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        var inviter = jsonModel.Inviter;
        if (inviter is not null)
            Inviter = new(inviter, client);

        var targetUser = jsonModel.TargetUser;
        if (targetUser is not null)
            TargetUser = new(targetUser, client);

        var targetApplication = jsonModel.TargetApplication;
        if (targetApplication is not null)
            TargetApplication = new(targetApplication, client);
    }

    public InviteType Type => _jsonModel.Type;

    public ulong ChannelId => _jsonModel.ChannelId;

    public string Code => _jsonModel.Code;

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public ulong? GuildId => _jsonModel.GuildId;

    public User? Inviter { get; }

    public int MaxAge => _jsonModel.MaxAge;

    public int MaxUses => _jsonModel.MaxUses;

    public InviteTargetType? TargetType => _jsonModel.TargetType;

    public User? TargetUser { get; }

    public Application? TargetApplication { get; }

    public bool Temporary => _jsonModel.Temporary;

    public int Uses => _jsonModel.Uses;

    ulong? IInvite.ChannelId => ChannelId;

    int? IInvite.MaxAge => MaxAge;

    int? IInvite.MaxUses => MaxUses;

    bool? IInvite.Temporary => Temporary;

    int? IInvite.Uses => Uses;

    DateTimeOffset? IInvite.CreatedAt => CreatedAt;
}
