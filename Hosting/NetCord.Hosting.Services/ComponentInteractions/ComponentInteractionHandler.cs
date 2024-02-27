using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

[GatewayEvent(nameof(GatewayClient.InteractionCreate))]
internal class ComponentInteractionHandler<TInteraction, TContext> : IGatewayEventHandler<Interaction>, IShardedGatewayEventHandler<Interaction>, IHttpInteractionHandler where TInteraction : ComponentInteraction where TContext : IComponentInteractionContext
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;
    private readonly ComponentInteractionService<TContext> _interactionService;
    private readonly Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? _createContext;
    private readonly Func<IExecutionResult, TInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> _handleResultAsync;
    private readonly GatewayClient? _client;

    public ComponentInteractionHandler(IServiceProvider services, ILogger<ComponentInteractionHandler<TInteraction, TContext>> logger, ComponentInteractionService<TContext> interactionService, IOptions<ComponentInteractionServiceOptions<TInteraction, TContext>> options, GatewayClient? client = null)
    {
        _services = services;
        _logger = logger;
        _interactionService = interactionService;
        var optionsValue = options.Value;
        _createContext = optionsValue.CreateContext ?? ContextHelper.CreateContextDelegate<TInteraction, GatewayClient?, TContext>();
        _handleResultAsync = optionsValue.HandleResultAsync;
        _client = client;
    }

    public ValueTask HandleAsync(Interaction interaction) => HandleInteractionAsync(interaction, _client);

    public ValueTask HandleAsync(GatewayClient client, Interaction interaction) => HandleInteractionAsync(interaction, client);

    private async ValueTask HandleInteractionAsync(Interaction interaction, GatewayClient? client)
    {
        if (interaction is not TInteraction tInteraction)
            return;

        var services = _services;
        var context = _createContext!(tInteraction, client, services);
        var result = await _interactionService.ExecuteAsync(context, services).ConfigureAwait(false);

        try
        {
            await _handleResultAsync(result, tInteraction, client, _logger, services).ConfigureAwait(false);
        }
        catch (Exception exceptionHandlerException)
        {
            _logger.LogError(exceptionHandlerException, "An exception occurred while handling the result");
        }
    }
}
