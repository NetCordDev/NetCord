namespace NetCord;

public partial class RestClient
{
    public class InteractionModule
    {
        private readonly BotClient _client;

        internal InteractionModule(BotClient client)
        {
            _client = client;
        }

        public Task EndAsync(DiscordId interactionId, string interactionToken, RequestOptions? options = null)
        {
            InteractionCallback callback = new(InteractionCallbackType.DeferredUpdateMessage);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, callback.Build(), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task EndWithModifyAsync(DiscordId interactionId, string interactionToken, InteractionMessage message, RequestOptions? options = null)
        {
            InteractionCallback callback = new(InteractionCallbackType.UpdateMessage, message);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, callback.Build(), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task EndWithReplyAsync(DiscordId interactionId, string interactionToken, InteractionMessage message, RequestOptions? options = null)
        {
            InteractionCallback callback = new(InteractionCallbackType.ChannelMessageWithSource, message);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, callback.Build(), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task EndWithThinkingStateAsync(DiscordId interactionId, string interactionToken, RequestOptions? options = null)
        {
            InteractionCallback callback = new(InteractionCallbackType.DeferredChannelMessageWithSource);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, callback.Build(), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task ModifyThinkingStateAsync(DiscordId interactionApplicationId, string interactionToken, Message message, RequestOptions? options = null)
        {
            return _client.Rest.SendRequestAsync(HttpMethod.Patch, message.Build(), $"/webhooks/{interactionApplicationId}/{interactionToken}/messages/@original", options);
        }

        public Task ModifyMessageAsync(DiscordId interactionApplicationId, string interactionToken, DiscordId messageId, Message message, RequestOptions? options = null)
        {
            return _client.Rest.SendRequestAsync(HttpMethod.Patch, message.Build(), $"/webhooks/{interactionApplicationId}/{interactionToken}/messages/{messageId}", options);
        }
    }
}