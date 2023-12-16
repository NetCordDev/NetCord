using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace MyBot;

[GatewayEvent(nameof(GatewayClient.MessageCreate))]
public class MessageCreateHandler : IGatewayEventHandler<Message>
{
    private readonly ILogger<MessageCreateHandler> _logger;

    public MessageCreateHandler(ILogger<MessageCreateHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync(Message message)
    {
        _logger.LogInformation("{}", message.Content);
        return default;
    }
}
