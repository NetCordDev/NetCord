using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildJoinRequest(JsonModels.JsonGuildJoinRequest jsonModel, RestClient client) : Entity, IJsonModel<JsonModels.JsonGuildJoinRequest>
{
    JsonModels.JsonGuildJoinRequest IJsonModel<JsonModels.JsonGuildJoinRequest>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;
    public GuildJoinRequestStatus ApplicationStatus => jsonModel.ApplicationStatus;
    //public DateTimeOffset CreatedAt => jsonModel.CreatedAt;
    public ulong GuildId => jsonModel.GuildId;
    public DateTimeOffset LastSeenAt => jsonModel.LastSeenAt;
    public string? RejectionReason => jsonModel.RejectionReason;
    public ulong UserId => jsonModel.UserId;
    public User User { get; } = new(jsonModel.User, client);
    public IReadOnlyList<VerificationField> FormResponses { get; } = jsonModel.FormResponses.Select(e => new VerificationField(e)).ToArray();
    public User ActionedByUser { get; } = new(jsonModel.ActionedByUser, client);
    public ulong ActionedAt => jsonModel.ActionedAt;
}
