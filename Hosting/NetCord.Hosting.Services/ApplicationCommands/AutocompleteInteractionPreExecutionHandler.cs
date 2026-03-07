using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class AutocompleteInteractionPreExecutionHandler<TContext> : IAutocompleteInteractionPreExecutionHandler<TContext>
    where TContext : IAutocompleteInteractionContext
{
    public ValueTask<PreExecutionResult> HandleAsync(TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        logger.LogDebug("Received an autocomplete for application command with name '{Name}'", context.Interaction.Data.Name);

        return new(PreExecutionResult.Continue);
    }
}
