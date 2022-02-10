namespace NetCord.Services.Interactions;

public class InteractionModule<TContext> : BaseInteractionModule<TContext> where TContext : InteractionContext
{
    public Task RespondAsync(InteractionCallback callback, RequestOptions? options = null) => Context.Interaction.SendResponseAsync(callback, options);
}