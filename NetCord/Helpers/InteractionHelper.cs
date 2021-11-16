using System.Text.Json;

namespace NetCord;

public static class InteractionHelper
{
    public static Task EndInteractionAsync(BotClient client, DiscordId interactionId, string interactionToken)
    {
        InteractionCallback callback = new(InteractionCallbackType.DeferredUpdateMessage);
        string serializedCallback = JsonSerializer.Serialize(callback);
        return CDN.SendAsync(HttpMethod.Post, serializedCallback, $"/interactions/{interactionId}/{interactionToken}/callback", client);
    }

    public static Task EndInteractionWithModifyAsync(BotClient client, DiscordId interactionId, string interactionToken, InteractionMessage message)
    {
        InteractionCallback callback = new(InteractionCallbackType.UpdateMessage, message);
        string serializedCallback = JsonSerializer.Serialize(callback);
        return CDN.SendAsync(HttpMethod.Post, serializedCallback, $"/interactions/{interactionId}/{interactionToken}/callback", client);
    }

    public static Task EndInteractionWithReplyAsync(BotClient client, DiscordId interactionId, string interactionToken, InteractionMessage message)
    {
        InteractionCallback callback = new(InteractionCallbackType.ChannelMessageWithSource, message);
        string serializedCallback = JsonSerializer.Serialize(callback);
        return CDN.SendAsync(HttpMethod.Post, serializedCallback, $"/interactions/{interactionId}/{interactionToken}/callback", client);
    }

    public static Task EndInteractionWithThinkingStateAsync(BotClient client, DiscordId interactionId, string interactionToken)
    {
        InteractionCallback callback = new(InteractionCallbackType.DeferredChannelMessageWithSource);
        string serializedCallback = JsonSerializer.Serialize(callback);
        return CDN.SendAsync(HttpMethod.Post, serializedCallback, $"/interactions/{interactionId}/{interactionToken}/callback", client);
    }

    public static Task ModifyThinkingStateAsync(BotClient client, DiscordId interactionApplicationId, string interactionToken, BuiltMessage message)
    {
        return CDN.SendAsync(HttpMethod.Patch, message, $"/webhooks/{interactionApplicationId}/{interactionToken}/messages/@original", client);
    }

    public static Task ModifyInteractionMessageAsync(BotClient client, DiscordId interactionApplicationId, string interactionToken, DiscordId messageId, BuiltMessage message)
    {
        return CDN.SendAsync(HttpMethod.Patch, message, $"/webhooks/{interactionApplicationId}/{interactionToken}/messages/{messageId}", client);
    }
}