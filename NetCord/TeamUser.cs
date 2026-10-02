using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a user that is a member of a team.
/// </summary>
public class TeamUser(JsonTeamUser jsonModel, RestClient client) : User(jsonModel.User, client)
{
    /// <summary>
    /// The user's membership state.
    /// </summary>
    public MembershipState MembershipState { get; } = jsonModel.MembershipState;

    /// <summary>
    /// The ID corresponding to the user's team.
    /// </summary>
    public ulong TeamId { get; } = jsonModel.TeamId;

    /// <summary>
    /// The user's role within their team.
    /// </summary>
    public TeamRole Role { get; } = jsonModel.Role;
}
