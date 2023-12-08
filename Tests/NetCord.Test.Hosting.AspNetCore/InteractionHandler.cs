using NetCord.Hosting;

namespace NetCord.Test.Hosting.AspNetCore;

internal class InteractionHandler : IHttpInteractionHandler
{
    private readonly ILogger<InteractionHandler> _logger;

    public InteractionHandler(ILogger<InteractionHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync(Interaction interaction)
    {
        _logger.LogInformation("Interaction received: {Interaction}", interaction);

        return default;
    }
}
