using System.Text.Json.Serialization;

namespace NetCord;

/// <summary>
/// Represents a <see cref="TeamUser"/>'s role within a team, excluding owners, which are represented in the <see cref="Team.OwnerId"/> property.
/// </summary>
/// <remarks>
/// <para>Each role inherits the properties of the role below it, in the order of <see cref="ReadOnly"/>, <see cref="Developer"/>, <see cref="Admin"/>.</para>
/// <para>Only owners can take destructive, irreversible actions like deleting team-owned apps or the team itself. Teams are limited to 1 owner.</para>
/// </remarks>
[JsonConverter(typeof(JsonConverters.SafeStringEnumConverter<TeamRole>))]
public enum TeamRole : sbyte
{
    /// <summary>
    /// Admins have similar access to owners, with the exception of destructive actions, such as deleting applications.
    /// </summary>
    [JsonPropertyName("admin")]
    Admin,

    /// <summary>
    /// <para>Developers have access to sensitive information, such as client secrets and public keys.</para>
    /// <para>They can also take some limited actions, such as configuring interaction endpoints, or resetting bot tokens.</para>
    /// <para>Developers cannot manage the team or its members.</para>
    /// </summary>
    [JsonPropertyName("developer")]
    Developer,

    /// <summary>
    /// <para>Read-only users can access information about a team and its applications, such as application IDs, and payout records.</para>
    /// <para>They can also invite private applications.</para>
    /// </summary>
    [JsonPropertyName("read-only")]
    ReadOnly,
}
