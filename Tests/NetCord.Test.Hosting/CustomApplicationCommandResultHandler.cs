using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

internal class CustomApplicationCommandResultHandler : ApplicationCommandResultHandler<ApplicationCommandContext>
{
    public override InteractionMessageProperties GetFailMessage(IFailResult failResult, ApplicationCommandContext context, IServiceProvider services)
    {
        var message = base.GetFailMessage(failResult, context, services);

        message.WithFlags(MessageFlags.Ephemeral);

        return message;
    }
}
