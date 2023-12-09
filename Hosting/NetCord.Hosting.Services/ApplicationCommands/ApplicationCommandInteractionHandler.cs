using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

[GatewayEvent(nameof(GatewayClient.InteractionCreate))]
internal class ApplicationCommandInteractionHandler<TInteraction, TContext> : IGatewayEventHandler<Interaction>, IShardedGatewayEventHandler<Interaction>, IHttpInteractionHandler where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ApplicationCommandInteractionHandler<TInteraction, TContext>> _logger;
    private readonly ApplicationCommandService<TContext> _applicationCommandService;
    private readonly Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? _createContext;
    private readonly Func<Exception, TInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> _handleExceptionAsync;
    private readonly GatewayClient? _client;

    public ApplicationCommandInteractionHandler(IServiceProvider services, ILogger<ApplicationCommandInteractionHandler<TInteraction, TContext>> logger, ApplicationCommandService<TContext> applicationCommandService, IOptions<ApplicationCommandServiceOptions<TInteraction, TContext>> options, GatewayClient? client = null)
    {
        _services = services;
        _logger = logger;
        _applicationCommandService = applicationCommandService;

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
                await _applicationCommandService.ExecuteAsync(context, services).ConfigureAwait(false);
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
