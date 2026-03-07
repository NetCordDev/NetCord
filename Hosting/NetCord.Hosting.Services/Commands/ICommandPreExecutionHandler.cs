using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public interface ICommandPreExecutionHandler<TContext>
    where TContext : ICommandContext
{
    public ValueTask<PreExecutionResult> HandleAsync(TContext context, GatewayClient client, ILogger logger, IServiceProvider services);
}
