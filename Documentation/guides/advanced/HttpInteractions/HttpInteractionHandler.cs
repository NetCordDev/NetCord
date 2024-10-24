using NetCord;
using NetCord.Hosting;

namespace MyBot;

public class HttpInteractionHandler(ILogger<HttpInteractionHandler> logger) : IHttpInteractionHandler
{
    public ValueTask HandleAsync(Interaction interaction)
    {
        logger.LogInformation("Received interaction: {}", interaction);
        return default;
    }
}
