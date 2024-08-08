using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

[GatewayEvent(nameof(GatewayClient.MessageCreate))]
internal unsafe partial class CommandHandler<TContext> : IGatewayEventHandler<Message>, IShardedGatewayEventHandler<Message> where TContext : ICommandContext
{
    public IServiceProvider Services { get; }

    private readonly ILogger _logger;
    private readonly CommandService<TContext> _commandService;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly delegate*<CommandHandler<TContext>, Message, GatewayClient, ValueTask> _handleAsync;
    private readonly Func<Message, GatewayClient, IServiceProvider, ValueTask<int>> _getPrefixLengthAsync;
    private readonly Func<Message, GatewayClient, IServiceProvider, TContext> _createContext;
    private readonly ICommandResultHandler<TContext> _resultHandler;
    private readonly GatewayClient? _client;

    public CommandHandler(IServiceProvider services,
                          ILogger<CommandHandler<TContext>> logger,
                          CommandService<TContext> commandService,
                          IOptions<CommandServiceOptions<TContext>> options,
                          GatewayClient? client = null)
    {
        Services = services;

        _logger = logger;
        _commandService = commandService;

        var optionsValue = options.Value;

        if (optionsValue.UseScopes)
        {
            _scopeFactory = services.GetService<IServiceScopeFactory>() ?? throw new InvalidOperationException($"'{nameof(IServiceScopeFactory)}' is not registered in the '{nameof(IServiceProvider)}', but it is required for using scopes.");
            _handleAsync = &HandleMessageWithScopeAsync;
        }
        else
            _handleAsync = &HandleMessageAsync;

        _getPrefixLengthAsync = GetGetPrefixLengthAsyncDelegate(optionsValue);
        _createContext = optionsValue.CreateContext ?? ContextHelper.CreateContextDelegate<Message, GatewayClient, TContext>();
        _resultHandler = optionsValue.ResultHandler;
        _client = client;
    }

    private static Func<Message, GatewayClient, IServiceProvider, ValueTask<int>> GetGetPrefixLengthAsyncDelegate(CommandServiceOptions<TContext> options)
    {
        var getPrefixLengthAsync = options.GetPrefixLengthAsync;
        if (getPrefixLengthAsync is not null)
            return getPrefixLengthAsync;

        var prefix = options.Prefix;
        var prefixes = options.Prefixes;
        if (prefix is not null)
        {
            if (prefixes is not null)
                throw new InvalidOperationException($"Both '{nameof(options.Prefix)}' and '{options.Prefixes}' cannot be set at the same time.");

            return (message, _, _) => new(!message.Author.IsBot && message.Content.StartsWith(prefix) ? prefix.Length : -1);
        }

        if (prefixes is not null)
        {
            var count = prefixes.Count;
            if (count == 1)
            {
                var firstPrefix = prefixes[0];
                return (message, _, _) => new(!message.Author.IsBot && message.Content.StartsWith(firstPrefix) ? firstPrefix.Length : -1);
            }
            else
                return (message, _, _) =>
                {
                    if (message.Author.IsBot)
                        return new(-1);

                    var content = message.Content;
                    for (var i = 0; i < count; i++)
                    {
                        var prefix = prefixes[i];
                        if (content.StartsWith(prefix))
                            return new(prefix.Length);
                    }

                    return new(-1);
                };
        }

        throw new InvalidOperationException($"Either '{nameof(options.Prefix)}', '{nameof(options.Prefixes)}' or '{nameof(options.GetPrefixLengthAsync)}' must be set.");
    }

    public ValueTask HandleAsync(Message message) => _handleAsync(this, message, _client!);

    public ValueTask HandleAsync(GatewayClient client, Message message) => _handleAsync(this, message, client);
}

internal partial class CommandHandler<TContext>
{
    private AsyncServiceScope CreateAsyncScope() => _scopeFactory!.CreateAsyncScope();

    private static async ValueTask HandleMessageWithScopeAsync(CommandHandler<TContext> handler, Message message, GatewayClient client)
    {
        var scope = handler.CreateAsyncScope();

        try
        {
            await handler.HandleMessageAsyncCore(message, client, scope.ServiceProvider).ConfigureAwait(false);
        }
        finally
        {
            await scope.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static ValueTask HandleMessageAsync(CommandHandler<TContext> handler, Message message, GatewayClient client)
    {
        return handler.HandleMessageAsyncCore(message, client, handler.Services);
    }

    private async ValueTask HandleMessageAsyncCore(Message message, GatewayClient client, IServiceProvider services)
    {
        var prefixLength = await _getPrefixLengthAsync(message, client, services).ConfigureAwait(false);
        if (prefixLength < 0)
            return;

        var context = _createContext(message, client, services);
        var result = await _commandService.ExecuteAsync(prefixLength, context, services).ConfigureAwait(false);

        try
        {
            await _resultHandler.HandleResultAsync(result, message, context, client, _logger, services).ConfigureAwait(false);
        }
        catch (Exception exceptionHandlerException)
        {
            _logger.LogError(exceptionHandlerException, "An exception occurred while handling the result");
        }
    }
}
