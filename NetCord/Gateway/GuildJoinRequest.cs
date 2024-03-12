using NetCord.Rest;

namespace NetCord.Gateway;
public class GuildJoinRequest : IJsonModel<JsonModels.JsonGuildJoinRequest>
{
    private readonly JsonModels.JsonGuildJoinRequest _jsonModel;
    JsonModels.JsonGuildJoinRequest IJsonModel<JsonModels.JsonGuildJoinRequest>.JsonModel => _jsonModel;
    public GuildJoinRequestStatus ApplicationStatus => _jsonModel.ApplicationStatus;
    public DateTimeOffset? CreatedAt => _jsonModel.CreatedAt;
    public ulong GuildId => _jsonModel.GuildId;
    public DateTimeOffset LastSeen => _jsonModel.LastSeen;
    public string? RejectionReason => _jsonModel.RejectionReason;
    public ulong UserId => _jsonModel.UserId;
    public User User { get; }
    public IReadOnlyList<VerificationField> FormResponses { get; }
    public User ActionedByUser { get; }
    public ulong ActionedAt => _jsonModel.ActionedAt;
    public GuildJoinRequest(JsonModels.JsonGuildJoinRequest jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new User(jsonModel.User, client);
        ActionedByUser = new User(jsonModel.ActionedByUser, client);
        FormResponses = _jsonModel.FormResponses.Select(e => new VerificationField(e)).ToList();
    }
}
