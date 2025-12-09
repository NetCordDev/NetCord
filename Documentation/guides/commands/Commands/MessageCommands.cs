using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Commands;

/// <summary>
/// Examples for the Message Commands guide.
/// </summary>
public class MessageCommands : ApplicationCommandModule<MessageCommandContext>
{
    // TODO: Add message command examples (context menu on messages)
    // - Basic message commands
    // - Accessing message content
    // - Responding to message commands
    
    [MessageCommand("Quote Message")]
    public string QuoteMessage(Message target)
    {
        return $"> {target.Content}\n\n- {target.Author.Username}";
    }
}
