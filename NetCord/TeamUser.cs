using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a user as a part of the team indicated by the <see cref="TeamId"/>.
/// </summary>
public class TeamUser(JsonModels.JsonTeamUser jsonModel, RestClient client) : User(jsonModel.User, client), IJsonModel<JsonModels.JsonTeamUser>
{
    JsonModels.JsonTeamUser IJsonModel<JsonModels.JsonTeamUser>.JsonModel => jsonModel;

    /// <summary>
    /// The membership state of the <see cref="TeamUser"/>.
    /// </summary>
    public MembershipState MembershipState => jsonModel.MembershipState;

    /// <summary>
    /// The ID of the <see cref="Team"/> the <see cref="TeamUser"/> belongs to.
    /// </summary>
    public ulong TeamId => jsonModel.TeamId;

    /// <summary>
    /// The role of the <see cref="TeamUser"/> in the associated <see cref="Team"/>.
    /// </summary>
    public TeamRole Role => jsonModel.Role;
}
