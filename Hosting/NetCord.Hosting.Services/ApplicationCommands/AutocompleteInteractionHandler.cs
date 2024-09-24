using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

[GatewayEvent(nameof(GatewayClient.InteractionCreate))]
internal unsafe partial class AutocompleteInteractionHandler<TInteraction,
                                                             TContext,
                                                             [DAM(DAMT.PublicConstructors)] TAutocompleteContext>
    : IGatewayEventHandler<Interaction>,
      IShardedGatewayEventHandler<Interaction>,
      IHttpInteractionHandler
    where TInteraction : ApplicationCommandInteraction
    where TContext : IApplicationCommandContext
    where TAutocompleteContext : IAutocompleteInteractionContext
{
    private IServiceProvider Services { get; }

    private readonly ILogger _logger;
    private readonly ApplicationCommandService<TContext, TAutocompleteContext> _applicationCommandService;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly delegate*<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>, Interaction, GatewayClient?, ValueTask> _handleAsync;
    private readonly Func<AutocompleteInteraction, GatewayClient?, IServiceProvider, TAutocompleteContext> _createContext;
    private readonly IAutocompleteInteractionResultHandler<TAutocompleteContext> _resultHandler;
    private readonly GatewayClient? _client;

    public AutocompleteInteractionHandler(IServiceProvider services,
                                          ILogger<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>> logger,
                                          ApplicationCommandService<TContext, TAutocompleteContext> applicationCommandService,
                                          IOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> options,
                                          GatewayClient? client = null)
    {
        Services = services;

        _logger = logger;
        _applicationCommandService = applicationCommandService;

        var optionsValue = options.Value;

        if (optionsValue.UseScopes.GetValueOrDefault(true))
        {
            _scopeFactory = services.GetService<IServiceScopeFactory>() ?? throw new InvalidOperationException($"'{nameof(IServiceScopeFactory)}' is not registered in the '{nameof(IServiceProvider)}', but it is required for using scopes.");
            _handleAsync = &HandleInteractionWithScopeAsync;
        }
        else
            _handleAsync = &HandleInteractionAsync;

        _createContext = optionsValue.CreateAutocompleteContext ?? ContextHelper.CreateContextDelegate<AutocompleteInteraction, GatewayClient?, TAutocompleteContext>();
        _resultHandler = optionsValue.AutocompleteResultHandler ?? new AutocompleteInteractionResultHandler<TAutocompleteContext>();
        _client = client;
    }

    public ValueTask HandleAsync(Interaction interaction) => _handleAsync(this, interaction, _client);

    public ValueTask HandleAsync(GatewayClient client, Interaction interaction) => _handleAsync(this, interaction, client);
}

internal partial class AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>
{
    private AsyncServiceScope CreateAsyncScope() => _scopeFactory!.CreateAsyncScope();

    private static async ValueTask HandleInteractionWithScopeAsync(AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext> handler, Interaction interaction, GatewayClient? client)
    {
        if (interaction is not AutocompleteInteraction autocompleteInteraction)
            return;

        var scope = handler.CreateAsyncScope();

        try
        {
            await handler.HandleInteractionAsyncCore(autocompleteInteraction, client, scope.ServiceProvider).ConfigureAwait(false);
        }
        finally
        {
            await scope.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static ValueTask HandleInteractionAsync(AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext> handler, Interaction interaction, GatewayClient? client)
    {
        if (interaction is not AutocompleteInteraction autocompleteInteraction)
            return default;

        return handler.HandleInteractionAsyncCore(autocompleteInteraction, client, handler.Services);
    }

    private async ValueTask HandleInteractionAsyncCore(AutocompleteInteraction interaction, GatewayClient? client, IServiceProvider services)
    {
        var context = _createContext(interaction, client, services);
        var result = await _applicationCommandService.ExecuteAutocompleteAsync(context, services).ConfigureAwait(false);

        try
        {
            await _resultHandler.HandleResultAsync(result, context, client, _logger, services).ConfigureAwait(false);
        }
        catch (Exception exceptionHandlerException)
        {
            _logger.LogError(exceptionHandlerException, "An exception occurred while handling the result");
        }
    }
}
