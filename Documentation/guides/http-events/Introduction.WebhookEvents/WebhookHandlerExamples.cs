using NetCord.Hosting.AspNetCore;
using NetCord.Rest;

namespace MyBot;

internal static class WebhookHandlerExamples
{
    private sealed class MyDbContext;

    public static void AddDelegateBasedHandlerScoped(IServiceCollection services)
    {
        services.AddWebhookHandler(WebhookEvent.ApplicationDeauthorized, (ApplicationDeauthorizedWebhookEventArgs args,
                                                                          MyDbContext dbContext) =>
        {
            // 'dbContext' is scoped to the handler
        }, ServiceLifetime.Scoped);
    }

    public static void AddClassBasedHandler(IServiceCollection services)
    {
        services.AddWebhookHandler<ApplicationDeauthorizedWebhookHandler>();
    }

    public static void AddClassBasedHandlerScoped(IServiceCollection services)
    {
        services.AddWebhookHandler<ApplicationDeauthorizedWebhookHandler>(ServiceLifetime.Scoped);
    }

    public static void AddAllClassBasedHandlers(IServiceCollection services)
    {
        services.AddWebhookHandlers(typeof(Program).Assembly);
    }

    public static void AddAllClassBasedHandlersScoped(IServiceCollection services)
    {
        services.AddWebhookHandlers(typeof(Program).Assembly, ServiceLifetime.Scoped);
    }
}
