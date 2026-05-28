namespace NetCord;

/// <summary>
/// Indicates how a command is invoked client-side.
/// </summary>
public enum ApplicationCommandType
{
    /// <summary>
    /// Slash command. Displayed when a user types <c>/</c> in chat.
    /// </summary>
    ChatInput = 1,

    /// <summary>
    /// UI-based. Displayed when right clicking or tapping on a user.
    /// </summary>
    User = 2,

    /// <summary>
    /// UI-based. Displayed when right clicking or tapping on a message.
    /// </summary>
    Message = 3,

    /// <summary>
    /// UI-based. Represents the primary way to invoke an app's Activity.
    /// </summary>
    EntryPoint = 4,
}
