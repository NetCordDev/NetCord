using NetCord;
using NetCord.Hosting;

namespace MyBot;

internal static class HttpInteractionHandlerExamples
{
    public static void AddDelegateBasedHandler(IServiceCollection services)
    {
        services.AddHttpInteractionHandler((Interaction interaction, ILogger<Program> logger) =>
        {
            logger.LogInformation("User '{Username}' triggered an interaction", interaction.User.Username);
        });
    }

    private sealed class MyDbContext;

    public static void AddDelegateBasedHandlerScoped(IServiceCollection services)
    {
        services.AddHttpInteractionHandler((Interaction interaction, MyDbContext dbContext) =>
        {
            // 'dbContext' is scoped to the handler
        }, ServiceLifetime.Scoped);
    }

    public static void AddClassBasedHandler(IServiceCollection services)
    {
        services.AddHttpInteractionHandler<HttpInteractionHandler>();
    }

    public static void AddClassBasedHandlerScoped(IServiceCollection services)
    {
        services.AddHttpInteractionHandler<HttpInteractionHandler>(ServiceLifetime.Scoped);
    }

    public static void AddAllClassBasedHandlers(IServiceCollection services)
    {
        services.AddHttpInteractionHandlers(typeof(Program).Assembly);
    }

    public static void AddAllClassBasedHandlersScoped(IServiceCollection services)
    {
        services.AddHttpInteractionHandlers(typeof(Program).Assembly, ServiceLifetime.Scoped);
    }
}
