namespace NetCord;

/// <summary>
/// Specifies a <see cref="TeamUser"/>'s state, relative to a team.
/// </summary>
public enum MembershipState
{
    /// <summary>
    /// The user has been invited to the team, but has not yet accepted or declined.
    /// </summary>
    Invited = 1,

    /// <summary>
    /// The user has accepted a team invitation, and is a member of the team.
    /// </summary>
    Accepted = 2,
}
