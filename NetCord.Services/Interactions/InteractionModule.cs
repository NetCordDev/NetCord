using NetCord.Rest;

namespace NetCord.Services.Interactions;

public class InteractionModule<TContext> : BaseInteractionModule<TContext> where TContext : InteractionContext
{
    public Task RespondAsync(InteractionCallback callback, RequestProperties? options = null) => Context.Interaction.SendResponseAsync(callback, options);
}