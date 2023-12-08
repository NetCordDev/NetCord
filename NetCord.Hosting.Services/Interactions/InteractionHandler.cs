using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.Interactions;

namespace NetCord.Hosting.Services.Interactions;

[GatewayEvent(nameof(GatewayClient.InteractionCreate))]
internal class InteractionHandler<TInteraction, TContext> : IGatewayEventHandler<Interaction>, IShardedGatewayEventHandler<Interaction>, IHttpInteractionHandler where TInteraction : Interaction where TContext : IInteractionContext
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;
    private readonly InteractionService<TContext> _interactionService;
    private readonly Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? _createContext;
    private readonly Func<Exception, TInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> _handleExceptionAsync;
    private readonly GatewayClient? _client;

    public InteractionHandler(IServiceProvider services, ILogger<InteractionHandler<TInteraction, TContext>> logger, InteractionService<TContext> interactionService, IOptions<InteractionServiceOptions<TInteraction, TContext>> options, GatewayClient? client = null)
    {
        _services = services;
        _logger = logger;
        _interactionService = interactionService;
        var optionsValue = options.Value;
        _createContext = optionsValue.CreateContext ?? ContextHelper.CreateContextDelegate<TInteraction, GatewayClient?, TContext>();
        _handleExceptionAsync = optionsValue.HandleExceptionAsync;
        _client = client;
    }

    public ValueTask HandleAsync(Interaction interaction) => HandleInteractionAsync(interaction, _client);

    public ValueTask HandleAsync(GatewayClient client, Interaction interaction) => HandleInteractionAsync(interaction, client);

    private async ValueTask HandleInteractionAsync(Interaction interaction, GatewayClient? client)
    {
        if (interaction is TInteraction tInteraction)
        {
            var services = _services;
            try
            {
                var context = _createContext!(tInteraction, client, services);
                await _interactionService.ExecuteAsync(context, services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                try
                {
                    await _handleExceptionAsync(ex, tInteraction, client, _logger, services).ConfigureAwait(false);
                }
                catch (Exception exceptionHandlerException)
                {
                    _logger.LogError(exceptionHandlerException, "An exception occurred while handling an exception");
                }
            }
        }
    }
}
