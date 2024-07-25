using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(Channel)], nameof(Channel.Id), Cast = true)]
    public async Task<Channel> GetChannelAsync(ulong channelId, RestRequestProperties? properties = null)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);

    [GenerateAlias([typeof(GroupDMChannel)], nameof(GroupDMChannel.Id), Cast = true)]
    public async Task<Channel> ModifyGroupDMChannelAsync(ulong channelId, Action<GroupDMChannelOptions> action, RestRequestProperties? properties = null)
    {
        GroupDMChannelOptions groupDMChannelOptions = new();
        action(groupDMChannelOptions);
        using (HttpContent content = new JsonContent<GroupDMChannelOptions>(groupDMChannelOptions, Serialization.Default.GroupDMChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id), Cast = true)]
    public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, Action<GuildChannelOptions> action, RestRequestProperties? properties = null)
    {
        GuildChannelOptions guildChannelOptions = new();
        action(guildChannelOptions);
        using (HttpContent content = new JsonContent<GuildChannelOptions>(guildChannelOptions, Serialization.Default.GuildChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(Channel)], nameof(Channel.Id), Cast = true)]
    public async Task<Channel> DeleteChannelAsync(ulong channelId, RestRequestProperties? properties = null)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public IAsyncEnumerable<RestMessage> GetMessagesAsync(ulong channelId, PaginationProperties<ulong>? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.Before, 100);

        return new QueryPaginationAsyncEnumerable<RestMessage, ulong>(
            this,
            paginationProperties,
            paginationProperties.Direction.GetValueOrDefault() switch
            {
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).Select(json => new RestMessage(json, this)),
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).GetReversedIEnumerable().Select(json => new RestMessage(json, this)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            m => m.Id,
            HttpMethod.Get,
            $"/channels/{channelId}/messages",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public async Task<IReadOnlyDictionary<ulong, RestMessage>> GetMessagesAroundAsync(ulong channelId, ulong messageId, int? limit = null, RestRequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages", $"?limit={limit.GetValueOrDefault(100)}&around={messageId}", new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).ToDictionary(m => m.Id, m => new RestMessage(m, this));

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<RestMessage> GetMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), TypeNameOverride = "Message")]
    public async Task<RestMessage> SendMessageAsync(ulong channelId, MessageProperties message, RestRequestProperties? properties = null)
    {
        using (HttpContent content = message.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(AnnouncementGuildChannel)], nameof(AnnouncementGuildChannel.Id))]
    [GenerateAlias([typeof(AnnouncementGuildThread)], nameof(AnnouncementGuildThread.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<RestMessage> CrosspostMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task AddMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", null, new(channelId), properties);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", null, new(channelId), properties);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, ulong userId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/{userId}", null, new(channelId), properties);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public IAsyncEnumerable<User> GetMessageReactionsAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, MessageReactionsPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.After, 100);

        var type = paginationProperties.Type;

        return new QueryPaginationAsyncEnumerable<User, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(Serialization.Default.JsonUserArray).ConfigureAwait(false)).Select(u => new User(u, this)),
            u => u.Id,
            HttpMethod.Get,
            $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), type.HasValue ? $"?type={(byte)type.GetValueOrDefault()}&" : "?"),
            new(channelId),
            properties);
    }

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteAllMessageReactionsAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", null, new(channelId), properties);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteAllMessageReactionsAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}", null, new(channelId), properties);

    private static string ReactionEmojiToString(ReactionEmojiProperties emoji) => emoji.Id.HasValue ? $"{emoji.Name}:{emoji.Id.GetValueOrDefault()}" : emoji.Name;

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<RestMessage> ModifyMessageAsync(ulong channelId, ulong messageId, Action<MessageOptions> action, RestRequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public Task DeleteMessagesAsync(ulong channelId, IEnumerable<ulong> messageIds, RestRequestProperties? properties = null)
    {
        var ids = new ulong[100];
        int c = 0;
        List<Task> tasks = [];
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
            tasks.Add(BulkDeleteMessagesAsync(channelId, new(ids, 0, c), properties));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], properties));
        return Task.WhenAll(tasks);
    }

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public async Task DeleteMessagesAsync(ulong channelId, IAsyncEnumerable<ulong> messageIds, RestRequestProperties? properties = null)
    {
        var ids = new ulong[100];
        int c = 0;
        List<Task> tasks = [];
        await foreach (var id in messageIds.ConfigureAwait(false))
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
            tasks.Add(BulkDeleteMessagesAsync(channelId, new(ids, 0, c), properties));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], properties));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task BulkDeleteMessagesAsync(ulong channelId, ArraySegment<ulong> messageIds, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<BulkDeleteMessagesProperties>(new(messageIds), Serialization.Default.BulkDeleteMessagesProperties))
            await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages/bulk-delete", null, new(channelId), properties).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public async Task ModifyGuildChannelPermissionsAsync(ulong channelId, PermissionOverwriteProperties permissionOverwrite, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<PermissionOverwriteProperties>(permissionOverwrite, Serialization.Default.PermissionOverwriteProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", null, new(channelId), properties).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public async Task<IEnumerable<RestInvite>> GetGuildChannelInvitesAsync(ulong channelId, RestRequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInviteArray).ConfigureAwait(false)).Select(r => new RestInvite(r, this));

    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public async Task<RestInvite> CreateGuildChannelInviteAsync(ulong channelId, InviteProperties? inviteProperties = null, RestRequestProperties? properties = null)
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
    {
        using (HttpContent content = new JsonContent<InviteProperties?>(inviteProperties, Serialization.Default.InviteProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/invites", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);
    }
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public Task DeleteGuildChannelPermissionAsync(ulong channelId, ulong overwriteId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", null, new(channelId), properties);

    [GenerateAlias([typeof(AnnouncementGuildChannel)], nameof(AnnouncementGuildChannel.Id))]
    [GenerateAlias([typeof(AnnouncementGuildThread)], nameof(AnnouncementGuildThread.Id), TypeNameOverride = nameof(AnnouncementGuildChannel))]
    public async Task<FollowedChannel> FollowAnnouncementGuildChannelAsync(ulong channelId, ulong webhookChannelId, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<FollowAnnouncementGuildChannelProperties>(new(webhookChannelId), Serialization.Default.FollowAnnouncementGuildChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/followers", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonFollowedChannel).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public Task TriggerTypingStateAsync(ulong channelId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", null, new(channelId), properties);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public async Task<IDisposable> EnterTypingStateAsync(ulong channelId, RestRequestProperties? properties = null)
    {
        TypingReminder typingReminder = new(channelId, this, properties);
        await typingReminder.StartAsync().ConfigureAwait(false);
        return typingReminder;
    }

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public async Task<IReadOnlyDictionary<ulong, RestMessage>> GetPinnedMessagesAsync(ulong channelId, RestRequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).ToDictionary(m => m.Id, m => new RestMessage(m, this));

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task PinMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", null, new(channelId), properties);

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task UnpinMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", null, new(channelId), properties);

    [GenerateAlias([typeof(GroupDMChannel)], nameof(GroupDMChannel.Id))]
    public async Task GroupDMChannelAddUserAsync(ulong channelId, ulong userId, GroupDMChannelUserAddProperties groupDMChannelUserAddProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GroupDMChannelUserAddProperties>(groupDMChannelUserAddProperties, Serialization.Default.GroupDMChannelUserAddProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/channels/{channelId}/recipients/{userId}", null, new(channelId), properties).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(GroupDMChannel)], nameof(GroupDMChannel.Id))]
    public Task GroupDMChannelDeleteUserAsync(ulong channelId, ulong userId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", null, new(channelId), properties);

    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, ulong messageId, GuildThreadFromMessageProperties threadFromMessageProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildThreadFromMessageProperties>(threadFromMessageProperties, Serialization.Default.GuildThreadFromMessageProperties))
            return GuildThread.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages/{messageId}/threads", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, GuildThreadProperties threadProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildThreadProperties>(threadProperties, Serialization.Default.GuildThreadProperties))
            return GuildThread.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/threads", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(ForumGuildChannel)], nameof(ForumGuildChannel.Id))]
    public async Task<ForumGuildThread> CreateForumGuildThreadAsync(ulong channelId, ForumGuildThreadProperties threadProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = threadProperties.Serialize())
            return new ForumGuildThread(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/threads", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public Task JoinGuildThreadAsync(ulong threadId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/@me", null, new(threadId), properties);

    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public Task AddGuildThreadUserAsync(ulong threadId, ulong userId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/{userId}", null, new(threadId), properties);

    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public Task LeaveGuildThreadAsync(ulong threadId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/@me", null, new(threadId), properties);

    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    [GenerateAlias([typeof(GuildThreadUser)], nameof(GuildThreadUser.ThreadId), nameof(GuildThreadUser.Id))]
    public Task DeleteGuildThreadUserAsync(ulong threadId, ulong userId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/{userId}", null, new(threadId), properties);

    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public async Task<ThreadUser> GetGuildThreadUserAsync(ulong threadId, ulong userId, bool withGuildUser = false, RestRequestProperties? properties = null)
    {
        var user = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members/{userId}", $"?with_member={withGuildUser}", new(threadId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonThreadUser).ConfigureAwait(false);
        return withGuildUser ? new GuildThreadUser(user, this) : new ThreadUser(user, this);
    }

    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public IAsyncEnumerable<ThreadUser> GetGuildThreadUsersAsync(ulong threadId, OptionalGuildUsersPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.After, 100);

        var withGuildUsers = paginationProperties.WithGuildUsers;

        return new QueryPaginationAsyncEnumerable<ThreadUser, ulong>(
            this,
            paginationProperties,
            withGuildUsers
                ? async s => (await s.ToObjectAsync(Serialization.Default.JsonThreadUserArray).ConfigureAwait(false)).Select(u => new GuildThreadUser(u, this))
                : async s => (await s.ToObjectAsync(Serialization.Default.JsonThreadUserArray).ConfigureAwait(false)).Select(u => new ThreadUser(u, this)),
            u => u.Id,
            HttpMethod.Get,
            $"/channels/{threadId}/thread-members",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), $"?with_member={withGuildUsers}&"),
            new(threadId),
            properties);
    }

    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    public IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(ulong channelId, PaginationProperties<DateTimeOffset>? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<DateTimeOffset>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.Before, 100);

        return new OptimizedQueryPaginationAsyncEnumerable<GuildThread, DateTimeOffset>(
            this,
            paginationProperties,
            async s =>
            {
                var result = await s.ToObjectAsync(Serialization.Default.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
                return (GuildThreadGenerator.CreateThreads(result, this), result.HasMore);
            },
            t => t.Metadata.ArchiveTimestamp,
            HttpMethod.Get,
            $"/channels/{channelId}/threads/archived/public",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), t => t.ToString("s")),
            new(channelId),
            properties);
    }

    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    public IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsAsync(ulong channelId, PaginationProperties<DateTimeOffset>? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<DateTimeOffset>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.Before, 100);

        return new OptimizedQueryPaginationAsyncEnumerable<GuildThread, DateTimeOffset>(
            this,
            paginationProperties,
            async s =>
            {
                var result = await s.ToObjectAsync(Serialization.Default.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
                return (GuildThreadGenerator.CreateThreads(result, this), result.HasMore);
            },
            t => t.Metadata.ArchiveTimestamp,
            HttpMethod.Get,
            $"/channels/{channelId}/threads/archived/private",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), t => t.ToString("s")),
            new(channelId),
            properties);
    }

    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsAsync(ulong channelId, PaginationProperties<ulong>? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.Before, 100);

        return new OptimizedQueryPaginationAsyncEnumerable<GuildThread, ulong>(
            this,
            paginationProperties,
            async s =>
            {
                var result = await s.ToObjectAsync(Serialization.Default.JsonRestGuildThreadPartialResult).ConfigureAwait(false);
                return (GuildThreadGenerator.CreateThreads(result, this), result.HasMore);
            },
            t => t.Id,
            HttpMethod.Get,
            $"/channels/{channelId}/threads/archived/private",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }
}
