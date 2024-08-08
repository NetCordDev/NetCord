using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandServiceOptions<TContext> where TContext : ICommandContext
{
    public CommandServiceConfiguration<TContext> Configuration { get; set; } = CommandServiceConfiguration<TContext>.Default;

    public bool UseScopes { get; set; } = true;

    public string? Prefix { get; set; }

    public IReadOnlyList<string>? Prefixes { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, ValueTask<int>>? GetPrefixLengthAsync { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, TContext>? CreateContext { get; set; }

    public ICommandResultHandler<TContext> ResultHandler { get; set; } = new CommandResultHandler<TContext>();
}
