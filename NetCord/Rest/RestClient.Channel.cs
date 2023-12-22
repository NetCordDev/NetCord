namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Channel> GetChannelAsync(ulong channelId, RequestProperties? properties = null)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);

    public async Task<Channel> ModifyGroupDMChannelAsync(ulong channelId, Action<GroupDMChannelOptions> action, RequestProperties? properties = null)
    {
        GroupDMChannelOptions groupDMChannelOptions = new();
        action(groupDMChannelOptions);
        using (HttpContent content = new JsonContent<GroupDMChannelOptions>(groupDMChannelOptions, Serialization.Default.GroupDMChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, Action<GuildChannelOptions> action, RequestProperties? properties = null)
    {
        GuildChannelOptions guildChannelOptions = new();
        action(guildChannelOptions);
        using (HttpContent content = new JsonContent<GuildChannelOptions>(guildChannelOptions, Serialization.Default.GuildChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<Channel> ModifyGuildThreadAsync(ulong channelId, Action<GuildThreadOptions> action, RequestProperties? properties = null)
    {
        GuildThreadOptions threadOptions = new();
        action(threadOptions);
        using (HttpContent content = new JsonContent<GuildThreadOptions>(threadOptions, Serialization.Default.GuildThreadOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<Channel> DeleteChannelAsync(ulong channelId, RequestProperties? properties = null)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);

    public IAsyncEnumerable<RestMessage> GetMessagesAsync(ulong channelId, PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.Before, 100);

        return new PaginationAsyncEnumerable<RestMessage, ulong>(
            this,
            paginationProperties,
            paginationProperties.Direction switch
            {
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).Select(json => new RestMessage(json, this)),
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).GetReversedIEnumerable().Select(json => new RestMessage(json, this)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            m => m.Id,
            HttpMethod.Get,
            $"/channels/{channelId}/messages",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }

    public async Task<IReadOnlyDictionary<ulong, RestMessage>> GetMessagesAroundAsync(ulong channelId, ulong messageId, int? limit = null, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages", $"?limit={limit.GetValueOrDefault(100)}&around={messageId}", new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).ToDictionary(m => m.Id, m => new RestMessage(m, this));

    public async Task<RestMessage> GetMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> SendMessageAsync(ulong channelId, MessageProperties message, RequestProperties? properties = null)
    {
        using (HttpContent content = message.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    public async Task<RestMessage> CrosspostMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    public Task AddMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", null, new(channelId), properties);

    public Task DeleteMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", null, new(channelId), properties);

    public Task DeleteMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/{userId}", null, new(channelId), properties);

    public IAsyncEnumerable<User> GetMessageReactionsAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.After, 100);

        return new PaginationAsyncEnumerable<User, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(Serialization.Default.JsonUserArray).ConfigureAwait(false)).Select(u => new User(u, this)),
            u => u.Id,
            HttpMethod.Get,
            $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }

    public Task DeleteAllMessageReactionsAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", null, new(channelId), properties);

    public Task DeleteAllMessageReactionsAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}", null, new(channelId), properties);

    private static string ReactionEmojiToString(ReactionEmojiProperties emoji) => emoji.Id.HasValue ? $"{emoji.Name}:{emoji.Id.GetValueOrDefault()}" : emoji.Name;

    public async Task<RestMessage> ModifyMessageAsync(ulong channelId, ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties);

    public Task DeleteMessagesAsync(ulong channelId, IEnumerable<ulong> messageIds, RequestProperties? properties = null)
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

    public async Task DeleteMessagesAsync(ulong channelId, IAsyncEnumerable<ulong> messageIds, RequestProperties? properties = null)
    {
        var ids = new ulong[100];
        int c = 0;
        List<Task> tasks = [];
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
            tasks.Add(BulkDeleteMessagesAsync(channelId, new(ids, 0, c), properties));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], properties));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task BulkDeleteMessagesAsync(ulong channelId, ArraySegment<ulong> messageIds, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<BulkDeleteMessagesProperties>(new(messageIds), Serialization.Default.BulkDeleteMessagesProperties))
            await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages/bulk-delete", null, new(channelId), properties).ConfigureAwait(false);
    }

    public async Task ModifyGuildChannelPermissionsAsync(ulong channelId, PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<PermissionOverwriteProperties>(permissionOverwrite, Serialization.Default.PermissionOverwriteProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", null, new(channelId), properties).ConfigureAwait(false);
    }

    public async Task<IEnumerable<RestGuildInvite>> GetGuildChannelInvitesAsync(ulong channelId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildInviteArray).ConfigureAwait(false)).Select(r => new RestGuildInvite(r, this));

    public async Task<RestGuildInvite> CreateGuildChannelInviteAsync(ulong channelId, GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null)
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
    {
        using (HttpContent content = new JsonContent<GuildInviteProperties?>(guildInviteProperties, Serialization.Default.GuildInviteProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/invites", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildInvite).ConfigureAwait(false), this);
    }
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

    public Task DeleteGuildChannelPermissionAsync(ulong channelId, ulong overwriteId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", null, new(channelId), properties);

    public async Task<FollowedChannel> FollowAnnouncementGuildChannelAsync(ulong channelId, ulong webhookChannelId, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<FollowAnnouncementGuildChannelProperties>(new(webhookChannelId), Serialization.Default.FollowAnnouncementGuildChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/followers", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonFollowedChannel).ConfigureAwait(false), this);
    }

    public Task TriggerTypingStateAsync(ulong channelId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", null, new(channelId), properties);

    public async Task<IDisposable> EnterTypingStateAsync(ulong channelId, RequestProperties? properties = null)
    {
        TypingReminder typingReminder = new(channelId, this, properties);
        await typingReminder.StartAsync().ConfigureAwait(false);
        return typingReminder;
    }

    public async Task<IReadOnlyDictionary<ulong, RestMessage>> GetPinnedMessagesAsync(ulong channelId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).ToDictionary(m => m.Id, m => new RestMessage(m, this));

    public Task PinMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", null, new(channelId), properties);

    public Task UnpinMessageAsync(ulong channelId, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", null, new(channelId), properties);

    public async Task GroupDMChannelAddUserAsync(ulong channelId, ulong userId, GroupDMUserAddProperties groupDMUserAddProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GroupDMUserAddProperties>(groupDMUserAddProperties, Serialization.Default.GroupDMUserAddProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/channels/{channelId}/recipients/{userId}", null, new(channelId), properties).ConfigureAwait(false);
    }

    public Task GroupDMChannelDeleteUserAsync(ulong channelId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", null, new(channelId), properties);

    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, ulong messageId, GuildThreadFromMessageProperties threadFromMessageProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildThreadFromMessageProperties>(threadFromMessageProperties, Serialization.Default.GuildThreadFromMessageProperties))
            return (GuildThread)Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages/{messageId}/threads", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, GuildThreadProperties threadProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildThreadProperties>(threadProperties, Serialization.Default.GuildThreadProperties))
            return (GuildThread)Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/threads", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<ForumGuildThread> CreateForumGuildThreadAsync(ulong channelId, ForumGuildThreadProperties threadProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = threadProperties.Serialize())
            return new ForumGuildThread(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/threads", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public Task JoinGuildThreadAsync(ulong threadId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/@me", null, new(threadId), properties);

    public Task AddGuildThreadUserAsync(ulong threadId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/{userId}", null, new(threadId), properties);

    public Task LeaveGuildThreadAsync(ulong threadId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/@me", null, new(threadId), properties);

    public Task DeleteGuildThreadUserAsync(ulong threadId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/{userId}", null, new(threadId), properties);

    public async Task<ThreadUser> GetGuildThreadUserAsync(ulong threadId, ulong userId, bool withGuildUser = false, RequestProperties? properties = null)
    {
        var user = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members/{userId}", $"?with_member={withGuildUser}", new(threadId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonThreadUser).ConfigureAwait(false);
        return withGuildUser ? new GuildThreadUser(user, this) : new ThreadUser(user, this);
    }

    public IAsyncEnumerable<ThreadUser> GetGuildThreadUsersAsync(ulong threadId, OptionalGuildUsersPaginationProperties? optionalGuildUsersPaginationProperties = null, RequestProperties? properties = null)
    {
        var withGuildUsers = optionalGuildUsersPaginationProperties is not null && optionalGuildUsersPaginationProperties.WithGuildUsers;

        var paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(optionalGuildUsersPaginationProperties, PaginationDirection.After, 100);

        return new PaginationAsyncEnumerable<ThreadUser, ulong>(
            this,
            paginationProperties,
            withGuildUsers
                ? async s => (await s.ToObjectAsync(Serialization.Default.JsonThreadUserArray).ConfigureAwait(false)).Select(u => new GuildThreadUser(u, this))
                : async s => (await s.ToObjectAsync(Serialization.Default.JsonThreadUserArray).ConfigureAwait(false)).Select(u => new ThreadUser(u, this)),
            u => u.Id,
            HttpMethod.Get,
            $"/channels/{threadId}/thread-members",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString(), $"?with_member={withGuildUsers}&"),
            new(threadId),
            properties);
    }

    public IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(ulong channelId, PaginationProperties<DateTimeOffset>? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<DateTimeOffset>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.Before, 100);

        return new OptimizedPaginationAsyncEnumerable<GuildThread, DateTimeOffset>(
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
            new(paginationProperties.Limit.GetValueOrDefault(), t => t.ToString("s")),
            new(channelId),
            properties);
    }

    public IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsAsync(ulong channelId, PaginationProperties<DateTimeOffset>? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<DateTimeOffset>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.Before, 100);

        return new OptimizedPaginationAsyncEnumerable<GuildThread, DateTimeOffset>(
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
            new(paginationProperties.Limit.GetValueOrDefault(), t => t.ToString("s")),
            new(channelId),
            properties);
    }

    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsAsync(ulong channelId, PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.Before, 100);

        return new OptimizedPaginationAsyncEnumerable<GuildThread, ulong>(
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
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }
}
