using NetCord.Gateway;
using NetCord.Rest;

namespace GettingStarted;

/// <summary>
/// Examples for the Your First Response guide.
/// </summary>
public static class YourFirstResponse
{
    // TODO: Add message handling examples
    // - Subscribing to message events
    // - Sending basic text responses
    // - Handling commands
    // - Error handling
    
    public static void SetupMessageHandler(GatewayClient client)
    {
        client.MessageCreate += async message =>
        {
            if (message.Content == "!ping")
            {
                await message.Channel!.SendMessageAsync(new MessageProperties
                {
                    Content = "Pong!"
                });
            }
        };
    }
}
