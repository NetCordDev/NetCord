using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace MyBot;

public class MessageCreateHandler(ILogger<MessageCreateHandler> logger) : IMessageCreateGatewayEventHandler
{
    public ValueTask HandleAsync(Message message)
    {
        logger.LogInformation("{}", message.Content);
        return default;
    }
}
