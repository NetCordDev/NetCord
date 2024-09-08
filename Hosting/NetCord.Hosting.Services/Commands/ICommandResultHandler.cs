using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public interface ICommandResultHandler<TContext> where TContext : ICommandContext
{
    public ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient client, ILogger logger, IServiceProvider services);
}
