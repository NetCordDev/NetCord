using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public interface IAutocompleteInteractionResultHandler<TAutocompleteContext> where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ValueTask HandleResultAsync(IExecutionResult result, TAutocompleteContext context, GatewayClient? client, ILogger logger, IServiceProvider services);
}
