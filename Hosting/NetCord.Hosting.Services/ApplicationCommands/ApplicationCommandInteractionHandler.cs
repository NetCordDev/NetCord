using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

internal unsafe partial class ApplicationCommandInteractionHandler<TInteraction, [DAM(DAMT.PublicConstructors)] TContext>
    : IInteractionCreateGatewayHandler,
      IInteractionCreateShardedGatewayHandler,
      IHttpInteractionHandler
    where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    private readonly IServiceProvider _services;
    private readonly IContextAccessor<TContext> _contextAccessor;
    private readonly ILogger _logger;
    private readonly ApplicationCommandService<TContext> _applicationCommandService;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly delegate*<ApplicationCommandInteractionHandler<TInteraction, TContext>, Interaction, GatewayClient?, ValueTask> _handleAsync;
    private readonly Func<TInteraction, GatewayClient?, IServiceProvider, TContext> _createContext;
    private readonly IApplicationCommandResultHandler<TContext> _resultHandler;
    private readonly IApplicationCommandPreExecutionHandler<TContext> _preExecutionHandler;
    private readonly GatewayClient? _client;

    public ApplicationCommandInteractionHandler(IServiceProvider services,
                                                IContextAccessor<TContext> contextAccessor,
                                                ILogger<ApplicationCommandInteractionHandler<TInteraction, TContext>> logger,
                                                ApplicationCommandService<TContext> applicationCommandService,
                                                IOptions<ApplicationCommandServiceOptions<TInteraction, TContext>> options,
                                                GatewayClient? client = null)
    {
        _services = services;
        _contextAccessor = contextAccessor;
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

        _createContext = optionsValue.CreateContext ?? ContextHelper.CreateContextDelegate<TInteraction, GatewayClient?, TContext>(_applicationCommandService.Configuration.ServiceResolverProvider);
        _resultHandler = optionsValue.ResultHandler ?? ApplicationCommandResultHandler<TContext>.Default;
        _preExecutionHandler = optionsValue.PreExecutionHandler ?? new ApplicationCommandPreExecutionHandler<TContext>();
        _client = client;
    }

    public ValueTask HandleAsync(Interaction interaction) => _handleAsync(this, interaction, _client);

    public ValueTask HandleAsync(GatewayClient client, Interaction interaction) => _handleAsync(this, interaction, client);
}

internal partial class ApplicationCommandInteractionHandler<TInteraction, TContext>
{
    private AsyncServiceScope CreateAsyncScope() => _scopeFactory!.CreateAsyncScope();

    private static async ValueTask HandleInteractionWithScopeAsync(ApplicationCommandInteractionHandler<TInteraction, TContext> handler, Interaction interaction, GatewayClient? client)
    {
        if (interaction is not TInteraction tInteraction)
            return;

        var scope = handler.CreateAsyncScope();

        try
        {
            await handler.HandleInteractionAsyncCore(tInteraction, client, scope.ServiceProvider).ConfigureAwait(false);
        }
        finally
        {
            await scope.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static ValueTask HandleInteractionAsync(ApplicationCommandInteractionHandler<TInteraction, TContext> handler, Interaction interaction, GatewayClient? client)
    {
        if (interaction is not TInteraction tInteraction)
            return default;

        return handler.HandleInteractionAsyncCore(tInteraction, client, handler._services);
    }

    private async ValueTask HandleInteractionAsyncCore(TInteraction interaction, GatewayClient? client, IServiceProvider services)
    {
        var context = _createContext(interaction, client, services);

        _contextAccessor.SetContext(context);

        PreExecutionResult preExecutionResult;
        try
        {
            preExecutionResult = await _preExecutionHandler.HandleAsync(context, client, _logger, services).ConfigureAwait(false);
        }
        catch (Exception preExecutionHandlerException)
        {
            _logger.LogError(preExecutionHandlerException, "An exception occurred while handling pre-execution, aborting execution");
            return;
        }

        if (preExecutionResult is SkipPreExecutionResult)
            return;

        var result = await _applicationCommandService.ExecuteAsync(context, services).ConfigureAwait(false);

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
