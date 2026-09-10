using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a user that is a member of a team.
/// </summary>
public class TeamUser(JsonModels.JsonTeamUser jsonModel, RestClient client) : User(jsonModel.User, client), IJsonModel<JsonModels.JsonTeamUser>
{
    JsonModels.JsonTeamUser IJsonModel<JsonModels.JsonTeamUser>.JsonModel => jsonModel;

    /// <summary>
    /// The user's membership state.
    /// </summary>
    public MembershipState MembershipState => jsonModel.MembershipState;

    /// <summary>
    /// The ID corresponding to the user's team.
    /// </summary>
    public ulong TeamId => jsonModel.TeamId;

    /// <summary>
    /// The user's role within their team.
    /// </summary>
    public TeamRole Role => jsonModel.Role;
}
