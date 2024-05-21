namespace NetCord.Rest;

/// <summary>
/// Indicates how a <see cref="GuildUser"/> joined a guild.
/// </summary>
public enum GuildUserJoinSourceType
{
    /// <summary>
    /// User joined via the 'Discovery' tab.
    /// </summary>
    GuildDiscovery = 3,

    /// <summary>
    /// User joined from a manually created invite link.
    /// </summary>
    GuildUserInvite = 5,

    /// <summary>
    /// User joined using the guild's vanity invite.
    /// </summary>
    GuildVanityInvite = 6,
}
