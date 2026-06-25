using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

internal class CustomApplicationCommandResultHandler : ApplicationCommandResultHandler<ApplicationCommandContext>
{
    public override async ValueTask<InteractionMessageProperties> GetFailMessageAsync(IFailResult failResult, ApplicationCommandContext context, IServiceProvider services)
    {
        var message = await base.GetFailMessageAsync(failResult, context, services).ConfigureAwait(false);

        message.WithFlags(MessageFlags.Ephemeral);

        return message;
    }
}
