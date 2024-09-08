namespace NetCord;

/// <summary>
/// The flags on a <see cref="GuildUser"/> object, mostly undocumented by Discord.
/// </summary>
[Flags]
public enum GuildUserFlags
{
    /// <summary>
    /// User has left and rejoined the guild.
    /// </summary>
    DidRejoin = 1 << 0,

    /// <summary>
    /// User has completed onboarding.
    /// </summary>
    CompletedOnboarding = 1 << 1,

    /// <summary>
    /// User bypasses guild verification requirements. Allows a user who does not meet verification requirements to participate in a guild.
    /// </summary>
    BypassesVerification = 1 << 2,

    /// <summary>
    /// User has started onboarding.
    /// </summary>
    StartedOnboarding = 1 << 3,

    /// <summary>
    /// No longer in use, undocumented.
    /// </summary>
    StartedHomeFlags = 1 << 5,

    /// <summary>
    /// No longer in use, undocumented.
    /// </summary>
    CompletedHomeFlags = 1 << 6,

    /// <summary>
    /// Undocumented.
    /// </summary>
    AutomodQuarantinedUsernameOrGuildNickname = 1 << 7,

    /// <summary>
    /// Undocumented.
    /// </summary>
    AutomodQuarantinedBio = 1 << 8,
}
