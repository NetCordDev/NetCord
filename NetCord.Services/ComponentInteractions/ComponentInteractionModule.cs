using NetCord.Rest;

namespace NetCord.Services.ComponentInteractions;

public class ComponentInteractionModule<TContext> : BaseComponentInteractionModule<TContext> where TContext : IComponentInteractionContext
{
    public Task RespondAsync(InteractionCallback callback, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.SendResponseAsync(callback, properties, cancellationToken);

    public Task<RestMessage> GetResponseAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.GetResponseAsync(properties, cancellationToken);

    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.ModifyResponseAsync(action, properties, cancellationToken);

    public Task DeleteResponseAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.DeleteResponseAsync(properties, cancellationToken);

    public Task<RestMessage> FollowupAsync(InteractionMessageProperties message, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.SendFollowupMessageAsync(message, properties, cancellationToken);

    public Task<RestMessage> GetFollowupAsync(ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.GetFollowupMessageAsync(messageId, properties, cancellationToken);

    public Task<RestMessage> ModifyFollowupAsync(ulong messageId, Action<MessageOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.ModifyFollowupMessageAsync(messageId, action, properties, cancellationToken);

    public Task DeleteFollowupAsync(ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.DeleteFollowupMessageAsync(messageId, properties, cancellationToken);
}
