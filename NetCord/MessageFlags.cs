namespace NetCord;

[Flags]
public enum MessageFlags : uint
{
    /// <summary>
    /// this message has been published to subscribed channels (via Channel Following)
    /// </summary>
    Crossposted = 1 << 0,
    /// <summary>
    /// this message originated from a message in another channel (via Channel Following)
    /// </summary>
    IsCrosspost = 1 << 1,
    /// <summary>
    /// do not include any embeds when serializing this message
    /// </summary>
    SuppressEmbeds = 1 << 2,
    /// <summary>
    /// the source message for this crosspost has been deleted (via Channel Following)
    /// </summary>
    SourceMessageDeleted = 1 << 3,
    /// <summary>
    /// this message came from the urgent message system
    /// </summary>
    Urgent = 1 << 4,
    /// <summary>
    /// this message has an associated thread, with the same id as the message
    /// </summary>
    HasThread = 1 << 5,
    /// <summary>
    /// this message is only visible to the user who invoked the Interaction
    /// </summary>
    Ephemeral = 1 << 6,
    /// <summary>
    /// this message is an Interaction Response and the bot is "thinking"
    /// </summary>
    Loading = 1 << 7,
}
