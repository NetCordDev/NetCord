using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildInvite : IJsonModel<JsonModels.JsonGuildInvite>
{
    JsonModels.JsonGuildInvite IJsonModel<JsonModels.JsonGuildInvite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildInvite _jsonModel;

    public GuildInvite(JsonModels.JsonGuildInvite jsonModel, RestClient client)
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

    public ulong ChannelId => _jsonModel.ChannelId;

    public string Code => _jsonModel.Code;

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public ulong? GuildId => _jsonModel.GuildId;

    public User? Inviter { get; }

    public int MaxAge => _jsonModel.MaxAge;

    public int MaxUses => _jsonModel.MaxUses;

    public GuildInviteTargetType? TargetType => _jsonModel.TargetType;

    public User? TargetUser { get; }

    public Application? TargetApplication { get; }

    public bool Temporary => _jsonModel.Temporary;

    public int Uses => _jsonModel.Uses;
}
