using System.Text.Json.Serialization;

namespace NetCord;

/// <summary>
/// A <see cref="TeamUser"/> can have one of four roles (Owner, <see cref="Admin"/> , <see cref="Developer"/>, and <see cref="ReadOnly"/>), and each role inherits the access of those below it.
/// </summary>
/// <remarks>
/// The Owner role is not represented in the <see cref="TeamRole"/> enum, as it is not represented in <see cref="TeamUser"/>'s <see cref="TeamUser.Role"/> field. Instead, owners can be identified using a <see cref="Team"/>'s <see cref="Team.OwnerId"/> field. They have the most permissive role, and can take destructive, irreversible actions like deleting team-owned apps or the team itself. Teams are limited to 1 owner.
/// </remarks>
[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<TeamRole>))]
public enum TeamRole
{
    /// <summary>
    /// Admins have similar access to owners, except they cannot take destructive actions on the team or team-owned apps.
    /// </summary>
    [JsonPropertyName("admin")]
    Admin,

    /// <summary>
    /// Developers can access information about team-owned apps, like the client secret or public key. They can also take limited actions on team-owned apps, like configuring interaction endpoints or resetting the bot token. <see cref="TeamUser"/>s with the <see cref="Developer"/> role cannot manage the <see cref="Team"/> or its users, or take destructive actions on team-owned <see cref="Application"/>s.
    /// </summary>
    [JsonPropertyName("developer")]
    Developer,

    /// <summary>
    /// Read-only users can access information about a <see cref="Team"/> and any team-owned <see cref="Application"/>s. Some examples include getting the IDs of applications and exporting payout records. Members can also invite bots associated with team-owned apps that are marked private.
    /// </summary>
    [JsonPropertyName("read-only")]
    ReadOnly,
}
