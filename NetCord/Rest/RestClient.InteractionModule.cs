namespace NetCord;

public partial class RestClient
{
    public partial class InteractionModule
    {
        private readonly RestClient _client;

        public ApplicationCommandModule ApplicationCommand { get; }

        internal InteractionModule(RestClient client)
        {
            _client = client;
            ApplicationCommand = new(client);
        }

        public Task SendResponseAsync(DiscordId interactionId, string interactionToken, InteractionCallback callback, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Post, callback.Build(), $"/interactions/{interactionId}/{interactionToken}/callback", options);

        public async Task<RestMessage> GetResponseAsync(DiscordId applicationId, string interactionToken, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), _client);

        public async Task<RestMessage> ModifyResponseAsync(DiscordId applicationId, string interactionToken, Action<MessageOptions> action, RequestOptions? options = null)
        {
            MessageOptions messageOptions = new();
            action(messageOptions);
            return new((await _client.SendRequestAsync(HttpMethod.Patch, messageOptions.Build(), $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), _client);
        }

        public Task DeleteResponseAsync(DiscordId applicationId, string interactionToken, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options);

        public async Task<RestMessage> SendFollowupMessageAsync(DiscordId applicationId, string interactionToken, InteractionMessage message, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Post, message.Build(), $"/webhooks/{applicationId}/{interactionToken}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), _client);

        public async Task<RestMessage> GetFollowupMessageAsync(DiscordId applicationId, string interactionToken, DiscordId messageId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), _client);

        public async Task<RestMessage> ModifyFollowupMessageAsync(DiscordId applicationId, string interactionToken, DiscordId messageId, Action<MessageOptions> action, RequestOptions? options = null)
        {
            MessageOptions messageOptions = new();
            action(messageOptions);
            return new((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(messageOptions), $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), _client);
        }

        public Task DeleteFollowupMessageAsync(DiscordId applicationId, string interactionToken, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options);
    }
}