using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Hosting.Services.Interactions;

public class InteractionServiceOptions<TInteraction, TContext> where TInteraction : Interaction where TContext : IInteractionContext
{
    public InteractionServiceConfiguration<TContext> Configuration { get; set; } = InteractionServiceConfiguration<TContext>.Default;

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public Func<Exception, TInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> HandleExceptionAsync { get; set; } = async (exception, interaction, client, logger, services) =>
    {
        logger.LogInformation(exception, "An exception occurred while executing an interaction of custom id '{CustomId}'", ((ICustomIdInteractionData)interaction.Data).CustomId);
        await interaction.SendResponseAsync(InteractionCallback.Message(exception.Message)).ConfigureAwait(false);
    };
}
