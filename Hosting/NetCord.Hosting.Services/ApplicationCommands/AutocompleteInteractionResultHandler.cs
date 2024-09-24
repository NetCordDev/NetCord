using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class AutocompleteInteractionResultHandler<TAutocompleteContext> : IAutocompleteInteractionResultHandler<TAutocompleteContext>
    where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ValueTask HandleResultAsync(IExecutionResult result, TAutocompleteContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var commandName = context.Interaction.Data.Name;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an autocomplete for application command of name '{Name}' failed with an exception", commandName);
        else
            logger.LogDebug("Execution of an autocomplete for application command of name '{Name}' failed with '{Message}'", commandName, failResult.Message);

        return default;
    }
}
