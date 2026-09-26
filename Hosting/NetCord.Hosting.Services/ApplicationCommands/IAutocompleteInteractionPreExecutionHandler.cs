using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public interface IAutocompleteInteractionPreExecutionHandler<TContext>
    where TContext : IAutocompleteInteractionContext
{
    public ValueTask<PreExecutionResult> HandleAsync(TContext context, GatewayClient? client, ILogger logger, IServiceProvider services);
}
