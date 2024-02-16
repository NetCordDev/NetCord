using NetCord.Hosting;

namespace NetCord.Test.Hosting.AspNetCore;

internal class InteractionHandler(ILogger<InteractionHandler> logger) : IHttpInteractionHandler
{
    public ValueTask HandleAsync(Interaction interaction)
    {
        logger.LogInformation("Interaction received: {Interaction}", interaction);

        return default;
    }
}
