using System.Text.Json;

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
            string serializedCallback = JsonSerializer.Serialize(callback);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, new JsonContent(serializedCallback), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task EndWithModifyAsync(DiscordId interactionId, string interactionToken, InteractionMessage message, RequestOptions? options = null)
        {
            InteractionCallback callback = new(InteractionCallbackType.UpdateMessage, message);
            string serializedCallback = JsonSerializer.Serialize(callback);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, new JsonContent(serializedCallback), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task EndWithReplyAsync(DiscordId interactionId, string interactionToken, InteractionMessage message, RequestOptions? options = null)
        {
            InteractionCallback callback = new(InteractionCallbackType.ChannelMessageWithSource, message);
            string serializedCallback = JsonSerializer.Serialize(callback);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, new JsonContent(serializedCallback), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task EndWithThinkingStateAsync(DiscordId interactionId, string interactionToken, RequestOptions? options = null)
        {
            InteractionCallback callback = new(InteractionCallbackType.DeferredChannelMessageWithSource);
            string serializedCallback = JsonSerializer.Serialize(callback);
            return _client.Rest.SendRequestAsync(HttpMethod.Post, new JsonContent(serializedCallback), $"/interactions/{interactionId}/{interactionToken}/callback", options);
        }

        public Task ModifyThinkingStateAsync(DiscordId interactionApplicationId, string interactionToken, BuiltMessage message, RequestOptions? options = null)
        {
            return _client.Rest.SendRequestAsync(HttpMethod.Patch, message._content, $"/webhooks/{interactionApplicationId}/{interactionToken}/messages/@original", options);
        }

        public Task ModifyMessageAsync(DiscordId interactionApplicationId, string interactionToken, DiscordId messageId, BuiltMessage message, RequestOptions? options = null)
        {
            return _client.Rest.SendRequestAsync(HttpMethod.Patch, message._content, $"/webhooks/{interactionApplicationId}/{interactionToken}/messages/{messageId}", options);
        }
    }
}