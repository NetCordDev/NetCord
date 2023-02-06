namespace NetCord;

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
    /// User bypasses guild verification requirements.
    /// </summary>
    BypassesVerification = 1 << 2,

    /// <summary>
    /// User has started onboarding.
    /// </summary>
    StartedOnboarding = 1 << 3,
}
