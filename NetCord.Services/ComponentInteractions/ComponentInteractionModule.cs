using NetCord.Rest;

namespace NetCord.Services.ComponentInteractions;

public class ComponentInteractionModule<TContext> : BaseComponentInteractionModule<TContext> where TContext : IComponentInteractionContext
{
    public Task RespondAsync(InteractionCallback callback, RestRequestProperties? properties = null) => Context.Interaction.SendResponseAsync(callback, properties);

    public Task<RestMessage> GetResponseAsync(RestRequestProperties? properties = null) => Context.Interaction.GetResponseAsync(properties);

    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RestRequestProperties? properties = null) => Context.Interaction.ModifyResponseAsync(action, properties);

    public Task DeleteResponseAsync(RestRequestProperties? properties = null) => Context.Interaction.DeleteResponseAsync(properties);

    public Task<RestMessage> FollowupAsync(InteractionMessageProperties message, RestRequestProperties? properties = null) => Context.Interaction.SendFollowupMessageAsync(message, properties);

    public Task<RestMessage> GetFollowupAsync(ulong messageId, RestRequestProperties? properties = null) => Context.Interaction.GetFollowupMessageAsync(messageId, properties);

    public Task<RestMessage> ModifyFollowupAsync(ulong messageId, Action<MessageOptions> action, RestRequestProperties? properties = null) => Context.Interaction.ModifyFollowupMessageAsync(messageId, action, properties);

    public Task DeleteFollowupAsync(ulong messageId, RestRequestProperties? properties = null) => Context.Interaction.DeleteFollowupMessageAsync(messageId, properties);
}
