using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

[GatewayEvent(nameof(GatewayClient.InteractionCreate))]
internal class AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext> : IGatewayEventHandler<Interaction>, IShardedGatewayEventHandler<Interaction>, IHttpInteractionHandler where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>> _logger;
    private readonly ApplicationCommandService<TContext, TAutocompleteContext> _applicationCommandService;
    private readonly Func<AutocompleteInteraction, GatewayClient?, IServiceProvider, TAutocompleteContext>? _createContext;
    private readonly Func<IExecutionResult, AutocompleteInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> _handleResultAsync;
    private readonly GatewayClient? _client;

    public AutocompleteInteractionHandler(IServiceProvider services, ILogger<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>> logger, ApplicationCommandService<TContext, TAutocompleteContext> applicationCommandService, IOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> options, GatewayClient? client = null)
    {
        _services = services;
        _logger = logger;
        _applicationCommandService = applicationCommandService;
        var optionsValue = options.Value;
        _createContext = optionsValue.CreateAutocompleteContext ?? ContextHelper.CreateContextDelegate<AutocompleteInteraction, GatewayClient?, TAutocompleteContext>();
        _handleResultAsync = optionsValue.HandleAutocompleteResultAsync;
        _client = client;
    }

    public ValueTask HandleAsync(Interaction interaction) => HandleInteractionAsync(interaction, _client);

    public ValueTask HandleAsync(GatewayClient client, Interaction interaction) => HandleInteractionAsync(interaction, client);

    private async ValueTask HandleInteractionAsync(Interaction interaction, GatewayClient? client)
    {
        if (interaction is not AutocompleteInteraction autocompleteInteraction)
            return;

        var services = _services;
        var context = _createContext!(autocompleteInteraction, client, services);
        var result = await _applicationCommandService.ExecuteAutocompleteAsync(context, services).ConfigureAwait(false);

        try
        {
            await _handleResultAsync(result, autocompleteInteraction, client, _logger, services).ConfigureAwait(false);
        }
        catch (Exception exceptionHandlerException)
        {
            _logger.LogError(exceptionHandlerException, "An exception occurred while handling the result");
        }
    }
}
