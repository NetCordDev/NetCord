using NetCord.Rest;

namespace NetCord.Services.ComponentInteractions;

/// <summary>
/// Represents a module for component interactions.
/// </summary>
/// <typeparam name="TContext">The context the invoked component interactions use.</typeparam>
public abstract class ComponentInteractionModule<TContext> : BaseComponentInteractionModule<TContext> where TContext : IComponentInteractionContext
{
    /// <inheritdoc cref="Interaction.SendResponseAsync" />
    public Task<InteractionCallbackResponse?> RespondAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.SendResponseAsync(callback, withResponse, properties, cancellationToken);

    /// <inheritdoc cref="Interaction.GetResponseAsync" />
    public Task<RestMessage> GetResponseAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.GetResponseAsync(properties, cancellationToken);

    /// <inheritdoc cref="Interaction.ModifyResponseAsync" />
    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.ModifyResponseAsync(action, properties, cancellationToken);

    /// <inheritdoc cref="Interaction.DeleteResponseAsync" />
    public Task DeleteResponseAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.DeleteResponseAsync(properties, cancellationToken);

    /// <inheritdoc cref="Interaction.SendFollowupMessageAsync" />
    public Task<RestMessage> FollowupAsync(InteractionMessageProperties message, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.SendFollowupMessageAsync(message, properties, cancellationToken);

    /// <inheritdoc cref="Interaction.GetFollowupMessageAsync" />
    public Task<RestMessage> GetFollowupAsync(ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.GetFollowupMessageAsync(messageId, properties, cancellationToken);

    /// <inheritdoc cref="Interaction.ModifyFollowupMessageAsync" />
    public Task<RestMessage> ModifyFollowupAsync(ulong messageId, Action<MessageOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.ModifyFollowupMessageAsync(messageId, action, properties, cancellationToken);

    /// <inheritdoc cref="Interaction.DeleteFollowupMessageAsync" />
    public Task DeleteFollowupAsync(ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => Context.Interaction.DeleteFollowupMessageAsync(messageId, properties, cancellationToken);
}
