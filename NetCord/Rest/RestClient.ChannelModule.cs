
using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
    public async Task<Channel> GetChannelAsync(DiscordId channelId, RequestProperties? options = null)
    {
        var json = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", options).ConfigureAwait(false))!;
        return Channel.CreateFromJson(json.ToObject<JsonChannel>(), this);
    }

    public async Task<Channel> ModifyChannelAsync(DiscordId channelId, Action<GroupDMChannelOptions> action, RequestProperties? options = null)
    {
        GroupDMChannelOptions groupDMChannelOptions = new();
        action(groupDMChannelOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, new JsonContent(groupDMChannelOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);
    }

    public async Task<Channel> DeleteChannelAsync(DiscordId channelId, RequestProperties? options = null)
        => Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);

    public async Task<DMChannel> GetDMChannelByUserIdAsync(DiscordId userId, RequestProperties? options = null)
        => new DMChannel((await SendRequestAsync(HttpMethod.Post, new JsonContent($"{{\"recipient_id\":\"{userId}\"}}"), "/users/@me/channels", options).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);

    public Task TriggerTypingStateAsync(DiscordId channelId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", options);

    public async Task<IDisposable> EnterTypingStateAsync(DiscordId channelId, RequestProperties? options = null)
    {
        TypingReminder typingReminder = new(channelId, this, options);
        await typingReminder.StartAsync().ConfigureAwait(false);
        return typingReminder;
    }

    public async Task<IReadOnlyDictionary<DiscordId, RestMessage>> GetPinnedMessagesAsync(DiscordId channelId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", options).ConfigureAwait(false))!.ToObject<JsonMessage[]>().ToDictionary(m => m.Id, m => new RestMessage(m, this));

    public Task PinMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", options);

    public Task UnpinMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", options);

    public Task GroupDMChannelAddUserAsync(DiscordId channelId, DiscordId userId, GroupDMUserAddProperties properties, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, new JsonContent(properties), $"/channels/{channelId}/recipients/{userId}", options);

    public Task GroupDMChannelDeleteUserAsync(DiscordId channelId, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", options);
}