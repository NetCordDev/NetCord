using NetCord.Hosting.AspNetCore;
using NetCord.Rest;

namespace MyBot;

public class ApplicationDeauthorizedWebhookHandler(
    ILogger<ApplicationDeauthorizedWebhookHandler> logger) : IApplicationDeauthorizedWebhookHandler
{
    public ValueTask HandleAsync(ApplicationDeauthorizedWebhookEventArgs args)
    {
        logger.LogInformation("User '{Username}' deauthorized", args.User.Username);

        return default;
    }
}
