using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

internal partial class CommandHandler<[DAM(DAMT.PublicConstructors)] TContext>
    : IMessageCreateGatewayHandler,
      IMessageCreateShardedGatewayHandler
    where TContext : ICommandContext
{
    private readonly IServiceProvider _services;
    private readonly IContextAccessor<TContext> _contextAccessor;
    private readonly ILogger _logger;
    private readonly CommandService<TContext> _commandService;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly Func<Message, GatewayClient, ValueTask> _handleAsync;
    private readonly Func<Message, GatewayClient, IServiceProvider, ValueTask<ReadOnlyMemory<char>?>> _getCommandTextAsync;
    private readonly Func<Message, GatewayClient, IServiceProvider, TContext> _createContext;
    private readonly ICommandResultHandler<TContext> _resultHandler;
    private readonly ICommandPreExecutionHandler<TContext> _preExecutionHandler;
    private readonly GatewayClient? _client;

    public CommandHandler(IServiceProvider services,
                          IContextAccessor<TContext> contextAccessor,
                          ILogger<CommandHandler<TContext>> logger,
                          CommandService<TContext> commandService,
                          IOptions<CommandServiceOptions<TContext>> options,
                          GatewayClient? client = null)
    {
        _services = services;
        _contextAccessor = contextAccessor;
        _logger = logger;
        _commandService = commandService;

        var optionsValue = options.Value;

        if (optionsValue.UseScopes.GetValueOrDefault(true))
        {
            _scopeFactory = services.GetService<IServiceScopeFactory>() ?? throw new InvalidOperationException($"'{nameof(IServiceScopeFactory)}' is not registered in the '{nameof(IServiceProvider)}', but it is required for using scopes.");
            _handleAsync = HandleMessageWithScopeAsync;
        }
        else
            _handleAsync = HandleMessageAsync;

        _getCommandTextAsync = GetGetCommandTextAsyncDelegate(optionsValue);
        _createContext = optionsValue.CreateContext ?? ContextHelper.CreateContextDelegate<Message, GatewayClient, TContext>(_commandService.Configuration.ServiceResolverProvider);
        _resultHandler = optionsValue.ResultHandler ?? new CommandResultHandler<TContext>();
        _preExecutionHandler = optionsValue.PreExecutionHandler ?? new CommandPreExecutionHandler<TContext>();
        _client = client;
    }

    private static Func<Message, GatewayClient, IServiceProvider, ValueTask<ReadOnlyMemory<char>?>> GetGetCommandTextAsyncDelegate(CommandServiceOptions<TContext> options)
    {
        var getPrefixLengthAsync = options.GetPrefixLengthAsync;
        var getCommandTextAsync = options.GetCommandTextAsync;

        if (getPrefixLengthAsync is not null)
        {
            if (getCommandTextAsync is not null)
                throw new InvalidOperationException($"Both '{nameof(options.GetPrefixLengthAsync)}' and '{nameof(options.GetCommandTextAsync)}' cannot be set at the same time.");

            return async (message, client, services) =>
            {
                var prefixLength = await getPrefixLengthAsync(message, client, services).ConfigureAwait(false);
                if (prefixLength < 0)
                    return null;

                return message.Content.AsMemory(prefixLength);
            };
        }

        if (getCommandTextAsync is not null)
            return getCommandTextAsync;

        var prefix = options.Prefix;
        var prefixes = options.Prefixes;

        if (prefix is not null)
        {
            if (prefixes is not null)
                throw new InvalidOperationException($"Both '{nameof(options.Prefix)}' and '{nameof(options.Prefixes)}' cannot be set at the same time.");

            return GetSimpleDelegate(prefix);
        }

        if (prefixes is not null)
        {
            var prefixesArray = prefixes.ToArray();

            if (prefixesArray.Length is 1)
                return GetSimpleDelegate(prefixesArray[0]);

            return (message, _, _) =>
            {
                if (!message.Author.IsBot)
                {
                    var content = message.Content;
                    var count = prefixesArray.Length;

                    for (var i = 0; i < count; i++)
                    {
                        var prefix = prefixesArray[i];
                        if (content.StartsWith(prefix))
                            return new(content.AsMemory(prefix.Length));
                    }
                }

                return default;
            };
        }

        throw new InvalidOperationException($"Either '{nameof(options.Prefix)}', '{nameof(options.Prefixes)}', '{nameof(options.GetPrefixLengthAsync)}' or '{nameof(options.GetCommandTextAsync)}' must be set.");

        static Func<Message, GatewayClient, IServiceProvider, ValueTask<ReadOnlyMemory<char>?>> GetSimpleDelegate(string prefix)
        {
            return (message, _, _) =>
            {
                if (!message.Author.IsBot && message.Content.StartsWith(prefix))
                    return new(message.Content.AsMemory(prefix.Length));

                return default;
            };
        }
    }

    public ValueTask HandleAsync(Message message) => _handleAsync(message, _client!);

    public ValueTask HandleAsync(GatewayClient client, Message message) => _handleAsync(message, client);

    private async ValueTask HandleMessageWithScopeAsync(Message message, GatewayClient client)
    {
        var scope = _scopeFactory!.CreateAsyncScope();

        try
        {
            await HandleMessageAsyncCore(message, client, scope.ServiceProvider).ConfigureAwait(false);
        }
        finally
        {
            await scope.DisposeAsync().ConfigureAwait(false);
        }
    }

    private ValueTask HandleMessageAsync(Message message, GatewayClient client)
    {
        return HandleMessageAsyncCore(message, client, _services);
    }

    private async ValueTask HandleMessageAsyncCore(Message message, GatewayClient client, IServiceProvider services)
    {
        var command = await _getCommandTextAsync(message, client, services).ConfigureAwait(false);
        if (!command.HasValue)
            return;

        var context = _createContext(message, client, services);

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

        var result = await _commandService.ExecuteAsync(command.GetValueOrDefault(), context, services).ConfigureAwait(false);

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
