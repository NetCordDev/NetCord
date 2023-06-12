using NetCord.Rest;

namespace NetCord.Services.Interactions;

public class InteractionModule<TContext> : BaseInteractionModule<TContext> where TContext : IInteractionContext
{
    public Task RespondAsync(InteractionCallback callback, RequestProperties? properties = null) => Context.Interaction.SendResponseAsync(callback, properties);

    public Task<RestMessage> GetResponseAsync(RequestProperties? properties = null) => Context.Interaction.GetResponseAsync(properties);

    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RequestProperties? properties = null) => Context.Interaction.ModifyResponseAsync(action, properties);

    public Task DeleteResponseAsync(RequestProperties? properties = null) => Context.Interaction.DeleteResponseAsync(properties);

    public Task<RestMessage> FollowupAsync(InteractionMessageProperties message, RequestProperties? properties = null) => Context.Interaction.SendFollowupMessageAsync(message, properties);

    public Task<RestMessage> GetFollowupAsync(ulong messageId, RequestProperties? properties = null) => Context.Interaction.GetFollowupMessageAsync(messageId, properties);

    public Task<RestMessage> ModifyFollowupAsync(ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null) => Context.Interaction.ModifyFollowupMessageAsync(messageId, action, properties);

    public Task DeleteFollowupAsync(ulong messageId, RequestProperties? properties = null) => Context.Interaction.DeleteFollowupMessageAsync(messageId, properties);
}
