using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandModule<TContext> : BaseApplicationCommandModule<TContext> where TContext : IApplicationCommandContext
{
    public Task RespondAsync(InteractionCallback callback, RequestProperties? options = null) => Context.Interaction.SendResponseAsync(callback, options);
}