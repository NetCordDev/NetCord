namespace NetCord;

/// <summary>
/// Represents a <see cref="ThreadUser"/>'s state, relative to a thread.
/// </summary>
[Flags]
public enum ThreadUserFlags
{
    /// <summary>
    /// User has had no thread interactions.
    /// </summary>
    None = 0,

    /// <summary>
    /// User has interacted with the thread.
    /// </summary>
    HasInteracted = 1 << 0,

    /// <summary>
    /// User receives notifcations for all messages.
    /// </summary>
    AllMessages = 1 << 1,

    /// <summary>
    /// User recieves notifications only for messages that mention them.
    /// </summary>
    OnlyMentions = 1 << 2,

    /// <summary>
    /// User does not receive any notifications.
    /// </summary>
    NoMessages = 1 << 3,
}
