using NetCord.JsonModels;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Channel> GetChannelAsync(ulong channelId, RequestProperties? properties = null)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);

    public async Task<Channel> ModifyGroupDMChannelAsync(ulong channelId, Action<GroupDMChannelOptions> action, RequestProperties? properties = null)
    {
        GroupDMChannelOptions groupDMChannelOptions = new();
        action(groupDMChannelOptions);
        using (HttpContent content = new JsonContent<GroupDMChannelOptions>(groupDMChannelOptions, GroupDMChannelOptions.GroupDMChannelOptionsSerializerContext.WithOptions.GroupDMChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}", new Route(RouteParameter.Channels), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, Action<GuildChannelOptions> action, RequestProperties? properties = null)
    {
        GuildChannelOptions guildChannelOptions = new();
        action(guildChannelOptions);
        using (HttpContent content = new JsonContent<GuildChannelOptions>(guildChannelOptions, GuildChannelOptions.GuildChannelOptionsSerializerContext.WithOptions.GuildChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}", new(RateLimits.RouteParameter.Channels), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<Channel> ModifyGuildThreadAsync(ulong channelId, Action<GuildThreadOptions> action, RequestProperties? properties = null)
    {
        GuildThreadOptions threadOptions = new();
        action(threadOptions);
        using (HttpContent content = new JsonContent<GuildThreadOptions>(threadOptions, GuildThreadOptions.GuildThreadOptionsSerializerContext.WithOptions.GuildThreadOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}", new(RateLimits.RouteParameter.Channels), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<Channel> DeleteChannelAsync(ulong channelId, RequestProperties? properties = null)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);

    public Task TriggerTypingStateAsync(ulong channelId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", new Route(RouteParameter.Typing), properties);

    public async Task<IDisposable> EnterTypingStateAsync(ulong channelId, RequestProperties? properties = null)
    {
        TypingReminder typingReminder = new(channelId, this, properties);
        await typingReminder.StartAsync().ConfigureAwait(false);
        return typingReminder;
    }

    public async Task<IReadOnlyDictionary<ulong, RestMessage>> GetPinnedMessagesAsync(ulong channelId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", new Route(RouteParameter.GetPinnedMessages), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageArraySerializerContext.WithOptions.JsonMessageArray).ConfigureAwait(false)).ToDictionary(m => m.Id, m => new RestMessage(m, this));

    public Task PinMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", new Route(RouteParameter.PinUnpinMessage), properties);

    public Task UnpinMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", new Route(RouteParameter.PinUnpinMessage), properties);

    public async Task GroupDMChannelAddUserAsync(ulong channelId, ulong userId, GroupDMUserAddProperties groupDMUserAddProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GroupDMUserAddProperties>(groupDMUserAddProperties, GroupDMUserAddProperties.GroupDMUserAddPropertiesSerializerContext.WithOptions.GroupDMUserAddProperties))
            await SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/recipients/{userId}", new Route(RouteParameter.Recipients, channelId), content, properties).ConfigureAwait(false);
    }

    public Task GroupDMChannelDeleteUserAsync(ulong channelId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", new Route(RouteParameter.Recipients, channelId), properties);

    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, ulong messageId, GuildThreadFromMessageProperties threadFromMessageProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildThreadFromMessageProperties>(threadFromMessageProperties, GuildThreadFromMessageProperties.GuildThreadFromMessagePropertiesSerializerContext.WithOptions.GuildThreadFromMessageProperties))
            return (GuildThread)Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/threads", new Route(RouteParameter.CreateGuildThread), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, GuildThreadProperties threadProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildThreadProperties>(threadProperties, GuildThreadProperties.GuildThreadPropertiesSerializerContext.WithOptions.GuildThreadProperties))
            return (GuildThread)Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/threads", new Route(RouteParameter.CreateGuildThread), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<ForumGuildThread> CreateForumGuildThreadAsync(ulong channelId, ForumGuildThreadProperties threadProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = threadProperties.Build())
            return new ForumGuildThread(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/threads", new Route(RouteParameter.CreateGuildThread), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public Task JoinGuildThreadAsync(ulong threadId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/@me", properties);

    public Task AddGuildThreadUserAsync(ulong threadId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/{userId}", properties);

    public Task LeaveGuildThreadAsync(ulong threadId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/@me", properties);

    public Task DeleteGuildThreadUserAsync(ulong threadId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/{userId}", properties);

    public async Task<ThreadUser> GetGuildThreadUserAsync(ulong threadId, ulong userId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members/{userId}", new Route(RouteParameter.GetGuildThreadUser), properties).ConfigureAwait(false)).ToObjectAsync(JsonThreadUser.JsonThreadUserSerializerContext.WithOptions.JsonThreadUser).ConfigureAwait(false), this);

    public async Task<IReadOnlyDictionary<ulong, ThreadUser>> GetGuildThreadUsersAsync(ulong threadId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members", new Route(RouteParameter.GetGuildThreadUsers), properties).ConfigureAwait(false)).ToObjectAsync(JsonThreadUser.JsonThreadUserArraySerializerContext.WithOptions.JsonThreadUserArray).ConfigureAwait(false)).ToDictionary(u => u.UserId, u => new ThreadUser(u, this));

    public async IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(ulong channelId, RequestProperties? properties = null)
    {
        var threads = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/public?limit=100", properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildThreadPartialResult.JsonRestGuildThreadPartialResultSerializerContext.WithOptions.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
        GuildThread? last = null;
        foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
            yield return last = thread;

        if (threads.HasMore)
        {
            await foreach (var t in GetPublicArchivedGuildThreadsBeforeAsync(channelId, last!.Metadata.ArchiveTimestamp, properties))
                yield return t;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsBeforeAsync(ulong channelId, DateTimeOffset before, RequestProperties? properties = null)
    {
        while (true)
        {
            GuildThread? last = null;
            var threads = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/public?limit=100&before={before:s}", properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildThreadPartialResult.JsonRestGuildThreadPartialResultSerializerContext.WithOptions.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
            foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
                yield return last = thread;

            if (!threads.HasMore)
                break;

            before = last!.Metadata.ArchiveTimestamp;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsAsync(ulong channelId, RequestProperties? properties = null)
    {
        var threads = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/private?limit=100", properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildThreadPartialResult.JsonRestGuildThreadPartialResultSerializerContext.WithOptions.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
        GuildThread? last = null;
        foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
            yield return last = thread;

        if (threads.HasMore)
        {
            await foreach (var t in GetPrivateArchivedGuildThreadsBeforeAsync(channelId, last!.Metadata.ArchiveTimestamp, properties))
                yield return t;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsBeforeAsync(ulong channelId, DateTimeOffset before, RequestProperties? properties = null)
    {
        while (true)
        {
            GuildThread? last = null;
            var threads = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/private?limit=100&before={before:s}", properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildThreadPartialResult.JsonRestGuildThreadPartialResultSerializerContext.WithOptions.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
            foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
                yield return last = thread;

            if (!threads.HasMore)
                break;

            before = last!.Metadata.ArchiveTimestamp;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsAsync(ulong channelId, RequestProperties? properties = null)
    {
        var threads = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/users/@me/threads/archived/private?limit=100", properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildThreadPartialResult.JsonRestGuildThreadPartialResultSerializerContext.WithOptions.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
        GuildThread? last = null;
        foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
            yield return last = thread;

        if (threads.HasMore)
        {
            await foreach (var t in GetJoinedPrivateArchivedGuildThreadsBeforeAsync(channelId, last!.Id, properties))
                yield return t;
        }
    }

    public async IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsBeforeAsync(ulong channelId, ulong before, RequestProperties? properties = null)
    {
        while (true)
        {
            GuildThread? last = null;
            var threads = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/users/@me/threads/archived/private?limit=100&before={before:s}", properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildThreadPartialResult.JsonRestGuildThreadPartialResultSerializerContext.WithOptions.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
            foreach (var thread in GuildThreadGenerator.CreateThreads(threads, this))
                yield return last = thread;

            if (!threads.HasMore)
                break;

            before = last!.Id;
        }
    }

    /// <summary>
    /// Sends a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <returns></returns>
    public async Task<RestMessage> SendMessageAsync(ulong channelId, MessageProperties message, RequestProperties? properties = null)
    {
        using (HttpContent content = message.Build())
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages", new Route(RouteParameter.SendMessage, channelId), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public async Task<RestMessage> CrosspostMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", new Route(RouteParameter.CrosspostMessage, channelId), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> ModifyMessageAsync(ulong channelId, ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Build())
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}/messages/{messageId}", new Route(RouteParameter.ModifyMessage), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", new Route(RouteParameter.DeleteMessage, channelId), properties);

    public Task DeleteMessagesAsync(ulong channelId, IEnumerable<ulong> messageIds, RequestProperties? properties = null)
    {
        var ids = new ulong[100];
        int c = 0;
        List<Task> tasks = new();
        foreach (var id in messageIds)
        {
            ids[c] = id;
            if (c == 99)
            {
                tasks.Add(BulkDeleteMessagesAsync(channelId, ids, properties));
                c = 0;
            }
            else
                c++;
        }
        if (c > 1)
            tasks.Add(BulkDeleteMessagesAsync(channelId, ids[..c], properties));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], properties));
        return Task.WhenAll(tasks);
    }

    public async Task DeleteMessagesAsync(ulong channelId, IAsyncEnumerable<ulong> messageIds, RequestProperties? properties = null)
    {
        var ids = new ulong[100];
        int c = 0;
        List<Task> tasks = new();
        await foreach (var id in messageIds)
        {
            ids[c] = id;
            if (c == 99)
            {
                tasks.Add(BulkDeleteMessagesAsync(channelId, ids, properties));
                c = 0;
            }
            else
                c++;
        }
        if (c > 1)
            tasks.Add(BulkDeleteMessagesAsync(channelId, ids[..c], properties));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], properties));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task BulkDeleteMessagesAsync(ulong channelId, ulong[] messageIds, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<BulkDeleteMessagesProperties>(new(messageIds), BulkDeleteMessagesProperties.BulkDeleteMessagesPropertiesSerializerContext.WithOptions.BulkDeleteMessagesProperties))
            await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/bulk-delete", new Route(RouteParameter.BulkDeleteMessages), content, properties).ConfigureAwait(false);
    }

    public async Task ModifyGuildChannelPermissionsAsync(ulong channelId, PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<PermissionOverwriteProperties>(permissionOverwrite, PermissionOverwriteProperties.PermissionOverwritePropertiesSerializerContext.WithOptions.PermissionOverwriteProperties))
            await SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", new(RouteParameter.ModifyDeleteGuildChannelPermissions), content, properties).ConfigureAwait(false);
    }

    public async Task<IEnumerable<RestGuildInvite>> GetGuildChannelInvitesAsync(ulong channelId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildInvite.JsonRestGuildInviteArraySerializerContext.WithOptions.JsonRestGuildInviteArray).ConfigureAwait(false)).Select(r => new RestGuildInvite(r, this));

    public async Task<RestGuildInvite> CreateGuildChannelInviteAsync(ulong channelId, GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null)
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
    {
        using (HttpContent content = new JsonContent<GuildInviteProperties?>(guildInviteProperties, GuildInviteProperties.GuildInvitePropertiesSerializerContext.WithOptions.GuildInviteProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/invites", new(RouteParameter.CreateGuildChannelInvite), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonRestGuildInvite.JsonRestGuildInviteSerializerContext.WithOptions.JsonRestGuildInvite).ConfigureAwait(false), this);
    }
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

    public Task DeleteGuildChannelPermissionAsync(ulong channelId, ulong overwriteId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", new Route(RouteParameter.ModifyDeleteGuildChannelPermissions), properties);

    public async Task<FollowedChannel> FollowAnnouncementGuildChannelAsync(ulong channelId, ulong webhookChannelId, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<FollowAnnouncementGuildChannelProperties>(new(webhookChannelId), FollowAnnouncementGuildChannelProperties.FollowAnnouncementGuildChannelPropertiesSerializerContext.WithOptions.FollowAnnouncementGuildChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/followers", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonFollowedChannel.JsonFollowedChannelSerializerContext.WithOptions.JsonFollowedChannel).ConfigureAwait(false), this);
    }

    public async Task<RestMessage> GetMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", new Route(RouteParameter.GetMessage), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);

    public async IAsyncEnumerable<RestMessage> GetMessagesAsync(ulong channelId, RequestProperties? properties = null)
    {
        byte messagesCount = 0;
        RestMessage? lastMessage = null;

        foreach (var message in await GetMaxMessagesAsyncTask(channelId, properties).ConfigureAwait(false))
        {
            yield return lastMessage = message;
            messagesCount++;
        }
        if (messagesCount == 100)
        {
            await foreach (var message in GetMessagesBeforeAsync(channelId, lastMessage!.Id, properties))
                yield return message;
        }
    }

    public async IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
    {
        byte messagesCount;
        do
        {
            messagesCount = 0;
            foreach (var message in await GetMaxMessagesBeforeAsyncTask(channelId, messageId, properties).ConfigureAwait(false))
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == 100);
    }

    public async IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
    {
        byte messagesCount;
        do
        {
            messagesCount = 0;
            foreach (var message in await GetMaxMessagesAfterAsyncTask(channelId, messageId, properties).ConfigureAwait(false))
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == 100);
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAsyncTask(ulong channelId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageArraySerializerContext.WithOptions.JsonMessageArray).ConfigureAwait(false)).Select(m => new RestMessage(m, this));

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAroundAsyncTask(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&around={messageId}", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageArraySerializerContext.WithOptions.JsonMessageArray).ConfigureAwait(false)).Select(m => new RestMessage(m, this));

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesBeforeAsyncTask(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&before={messageId}", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageArraySerializerContext.WithOptions.JsonMessageArray).ConfigureAwait(false)).Select(m => new RestMessage(m, this));

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAfterAsyncTask(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&after={messageId}", new Route(RouteParameter.GetMessages), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageArraySerializerContext.WithOptions.JsonMessageArray).ConfigureAwait(false)).Select(m => new RestMessage(m, this)).Reverse();

    public Task AddMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", new Route(RouteParameter.AddRemoveMessageReaction, channelId), properties);

    public Task DeleteMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/{userId}", new Route(RouteParameter.AddRemoveMessageReaction, channelId), properties);

    public async IAsyncEnumerable<User> GetMessageReactionsAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
    {
        var strEmoji = ReactionEmojiToString(emoji);
        byte count = 0;
        User? lastUser = null;

        foreach (var user in await GetMaxMessageReactionsAsyncTask(channelId, messageId, strEmoji, properties).ConfigureAwait(false))
        {
            yield return lastUser = user;
            count++;
        }
        if (count == 100)
        {
            await foreach (var user in GetMessageReactionsAfterAsync(channelId, messageId, strEmoji, lastUser!.Id, properties))
                yield return user;
        }
    }

    public async IAsyncEnumerable<User> GetMessageReactionsAfterAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, ulong userId, RequestProperties? properties = null)
    {
        var strEmoji = ReactionEmojiToString(emoji);
        byte count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxMessageReactionsAfterAsyncTask(channelId, messageId, strEmoji, userId, properties).ConfigureAwait(false))
            {
                yield return user;
                userId = user.Id;
                count++;
            }
        }
        while (count == 100);
    }

    private async Task<IEnumerable<User>> GetMaxMessageReactionsAsyncTask(ulong channelId, ulong messageId, string emoji, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}?limit=100", new Route(RouteParameter.GetMessageReactions), properties).ConfigureAwait(false)).ToObjectAsync(JsonUser.JsonUserArraySerializerContext.WithOptions.JsonUserArray).ConfigureAwait(false)).Select(u => new User(u, this));

    private async Task<IEnumerable<User>> GetMaxMessageReactionsAfterAsyncTask(ulong channelId, ulong messageId, string emoji, ulong after, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}?limit=100&after={after}", new Route(RouteParameter.GetMessageReactions), properties).ConfigureAwait(false)).ToObjectAsync(JsonUser.JsonUserArraySerializerContext.WithOptions.JsonUserArray).ConfigureAwait(false)).Select(u => new User(u, this));

    public Task DeleteAllMessageReactionsAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}", new Route(RouteParameter.GetMessageReactions), properties);

    public Task DeleteAllMessageReactionsAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", new Route(RouteParameter.GetMessageReactions), properties);

    private static string ReactionEmojiToString(ReactionEmojiProperties emoji) => emoji.EmojiType == ReactionEmojiType.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id;
}
