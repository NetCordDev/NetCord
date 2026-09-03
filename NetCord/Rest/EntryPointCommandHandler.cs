namespace NetCord.Rest;

public enum EntryPointCommandHandler
{
    /// <summary>
    /// The application handles the interaction using an interaction token.
    /// </summary>
    ApplicationHandler = 1,

    /// <summary>
    /// Discord handles the interaction by launching an Activity and sending a follow-up message without coordinating with the application.
    /// </summary>
    DiscordLaunchActivity = 2,
}
