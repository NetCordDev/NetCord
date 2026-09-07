using NetCord.Hosting.AspNetCore;
using NetCord.Rest;

namespace MyBot;

public class ApplicationAuthorizedWebhookHandler(
    ILogger<ApplicationAuthorizedWebhookHandler> logger) : IApplicationAuthorizedWebhookHandler
{
    public ValueTask HandleAsync(ApplicationAuthorizedWebhookEventArgs args)
    {
        logger.LogInformation("User '{Username}' authorized with scopes: {Scopes}",
                              args.User.Username,
                              args.Scopes);

        return default;
    }
}
