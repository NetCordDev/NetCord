using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;

namespace Commands;

/// <summary>
/// Examples for the Text Commands guide.
/// </summary>
public class TextCommands : IModule<GatewayMessageContext>
{
    // TODO: Add text command examples (prefix-based commands)
    // - Basic text commands
    // - Command parsing
    // - Argument handling
    // - Prefix configuration
    
    [Command("ping")]
    public string Ping()
    {
        return "Pong!";
    }
    
    [Command("say")]
    public string Say(string message)
    {
        return message;
    }
}
