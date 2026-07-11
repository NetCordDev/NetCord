using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    /// <summary>
    /// Retrieves a channel by its ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel to retrieve.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(Channel)], nameof(Channel.Id), Cast = true)]
    public async Task<Channel> GetChannelAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);

    /// <summary>
    /// Modifies a group DM channel’s properties.
    /// </summary>
    /// <param name="channelId">The ID of the group DM channel to modify.</param>
    /// <param name="action">An action delegate used to configure the channel’s updated properties.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GroupDMChannel)], nameof(GroupDMChannel.Id), Cast = true)]
    public async Task<Channel> ModifyGroupDMChannelAsync(ulong channelId, Action<GroupDMChannelOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GroupDMChannelOptions groupDMChannelOptions = new();
        action(groupDMChannelOptions);
        using (HttpContent content = new JsonContent<GroupDMChannelOptions>(groupDMChannelOptions, Serialization.Default.GroupDMChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Modifies a guild channel’s configuration.
    /// </summary>
    /// <param name="channelId">The ID of the guild channel to modify.</param>
    /// <param name="action">An action delegate used to configure the updated channel options.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id), Cast = true)]
    public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, Action<GuildChannelOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildChannelOptions guildChannelOptions = new();
        action(guildChannelOptions);
        using (HttpContent content = new JsonContent<GuildChannelOptions>(guildChannelOptions, Serialization.Default.GuildChannelOptions))
            return Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Sets a guild voice channel's status.
    /// </summary>
    /// <param name="channelId">The ID of the guild voice channel to update.</param>
    /// <param name="statusProperties">The status to apply to the channel.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(VoiceGuildChannel)], nameof(VoiceGuildChannel.Id))]
    public async Task SetVoiceGuildChannelStatusAsync(ulong channelId, VoiceGuildChannelStatusProperties statusProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<VoiceGuildChannelStatusProperties>(statusProperties, Serialization.Default.VoiceGuildChannelStatusProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/channels/{channelId}/voice-status", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel to delete.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(Channel)], nameof(Channel.Id), Cast = true)]
    public async Task<Channel> DeleteChannelAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);

    /// <summary>
    /// Retrieves an <see cref="IAsyncEnumerable{T}"/>, representing the messages of a specific channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel to retrieve messages from.</param>
    /// <param name="paginationProperties">Optional properties to customize result pagination, can be <see langword="null"/>.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }

    /// <summary>
    /// Retrieves a list of messages around a specific message ID within a channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel to retrieve messages from.</param>
    /// <param name="messageId">The ID of the message to center the result around.</param>
    /// <param name="limit">The maximum number of messages to retrieve, or <see langword="null"/> to use the default.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public async Task<IReadOnlyList<RestMessage>> GetMessagesAroundAsync(ulong channelId, ulong messageId, int? limit = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages", $"?limit={limit.GetValueOrDefault(100)}&around={messageId}", new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).Select(m => new RestMessage(m, this)).ToArray();

    /// <summary>
    /// Performs a search within a guild according to a set of specified filters.
    /// </summary>
    /// <param name="guildId">The ID of the guild to search through.</param>
    /// <param name="paginationProperties">Optional properties to customize result pagination and filtering, can be <see langword="null"/>.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <exception cref="InvalidOperationException"/>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public IAsyncEnumerable<GuildMessageSearchResult> SearchGuildMessagesAsync(ulong guildId, GuildMessagesSearchPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = GuildMessagesSearchPaginationProperties.PrepareWithOffset(paginationProperties, 0, PaginationDirection.After, 25);

        return new RetryingPartialResultsQueryPaginationAsyncEnumerable<GuildMessageSearchResult, int>(
            this,
            paginationProperties,
            async streamParam =>
            {
                var jsonModel = await streamParam.ToObjectAsync(Serialization.Default.JsonGuildMessagesSearchResult).ConfigureAwait(false);

                if (jsonModel.Messages is { } messages)
                {
                    GuildMessagesSearchResultData data = new(jsonModel, this);
                    return (messages.SelectMany(nested => nested.Select(m => new GuildMessageSearchResult.Success(new(m, this), data))), false);
                }

                return ([new GuildMessageSearchResult.Indexing(jsonModel)], true);
            },
            HttpMethod.Get,
            $"/guilds/{guildId}/messages/search",
            new(paginationProperties.BatchSize.GetValueOrDefault(), "offset", id => id.ToString(), CreateBaseQuery(paginationProperties)),
            new(guildId),
            properties);

        [SkipLocalsInit]
        static string CreateBaseQuery(GuildMessagesSearchPaginationProperties paginationProperties)
        {
            DefaultInterpolatedStringHandler handler = new(0, 0, null, stackalloc char[512]);

            handler.AppendLiteral("?");

            var direction = paginationProperties.Direction.GetValueOrDefault();

            AppendQueryParameter(ref handler, "sort_order", direction switch
            {
                PaginationDirection.Before => "asc",
                PaginationDirection.After => "desc",
                _ => InvalidEnum(nameof(paginationProperties.Direction)),
            });

            if (paginationProperties.SortBy is { } sortBy)
            {
                AppendQueryParameter(ref handler, "sort_by", sortBy switch
                {
                    GuildMessagesSearchSortingMode.Relevance => "relevance",
                    GuildMessagesSearchSortingMode.Timestamp => "timestamp",
                    _ => InvalidEnum(nameof(paginationProperties.SortBy)),
                });
            }

            if (paginationProperties.Content is { } content)
                AppendQueryParameter(ref handler, "content", Uri.EscapeDataString(content));

            if (paginationProperties.Slop is { } slop)
                AppendQueryParameterT(ref handler, "slop", slop);

            if (paginationProperties.Contents is { } contents)
            {
                foreach (var contentsItem in contents)
                    AppendQueryParameter(ref handler, "contents", Uri.EscapeDataString(contentsItem));
            }

            if (paginationProperties.AuthorIds is { } authorIds)
            {
                foreach (var authorId in authorIds)
                    AppendQueryParameterT(ref handler, "author_id", authorId);
            }

            if (paginationProperties.AuthorTypes is { } authorTypes)
            {
                const string AuthorTypeName = "author_type";

                if (authorTypes.HasFlag(GuildMessagesSearchAuthorTypes.User))
                    AppendQueryParameter(ref handler, AuthorTypeName, "user");
                if (authorTypes.HasFlag(GuildMessagesSearchAuthorTypes.Bot))
                    AppendQueryParameter(ref handler, AuthorTypeName, "bot");
                if (authorTypes.HasFlag(GuildMessagesSearchAuthorTypes.Webhook))
                    AppendQueryParameter(ref handler, AuthorTypeName, "webhook");

                if (authorTypes.HasFlag(GuildMessagesSearchAuthorTypes.NoUser))
                    AppendQueryParameter(ref handler, AuthorTypeName, "-user");
                if (authorTypes.HasFlag(GuildMessagesSearchAuthorTypes.NoBot))
                    AppendQueryParameter(ref handler, AuthorTypeName, "-bot");
                if (authorTypes.HasFlag(GuildMessagesSearchAuthorTypes.NoWebhook))
                    AppendQueryParameter(ref handler, AuthorTypeName, "-webhook");
            }

            if (paginationProperties.Mentions is { } mentions)
            {
                foreach (var mention in mentions)
                    AppendQueryParameterT(ref handler, "mentions", mention);
            }

            if (paginationProperties.MentionEveryone is { } mentionEveryone)
                AppendQueryParameterT(ref handler, "mention_everyone", mentionEveryone);

            if (paginationProperties.MinId is { } minId)
                AppendQueryParameterT(ref handler, "min_id", minId);

            if (paginationProperties.MaxId is { } maxId)
                AppendQueryParameterT(ref handler, "max_id", maxId);

            if (paginationProperties.Has is { } has)
            {
                const string HasName = "has";

                if (has.HasFlag(GuildMessagesSearchHasOptions.Link))
                    AppendQueryParameter(ref handler, HasName, "link");
                if (has.HasFlag(GuildMessagesSearchHasOptions.Embed))
                    AppendQueryParameter(ref handler, HasName, "embed");
                if (has.HasFlag(GuildMessagesSearchHasOptions.File))
                    AppendQueryParameter(ref handler, HasName, "file");
                if (has.HasFlag(GuildMessagesSearchHasOptions.Image))
                    AppendQueryParameter(ref handler, HasName, "image");
                if (has.HasFlag(GuildMessagesSearchHasOptions.Video))
                    AppendQueryParameter(ref handler, HasName, "video");
                if (has.HasFlag(GuildMessagesSearchHasOptions.Sound))
                    AppendQueryParameter(ref handler, HasName, "sound");
                if (has.HasFlag(GuildMessagesSearchHasOptions.Sticker))
                    AppendQueryParameter(ref handler, HasName, "sticker");
                if (has.HasFlag(GuildMessagesSearchHasOptions.Poll))
                    AppendQueryParameter(ref handler, HasName, "poll");
                if (has.HasFlag(GuildMessagesSearchHasOptions.Snapshot))
                    AppendQueryParameter(ref handler, HasName, "snapshot");

                if (has.HasFlag(GuildMessagesSearchHasOptions.NoLink))
                    AppendQueryParameter(ref handler, HasName, "-link");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoEmbed))
                    AppendQueryParameter(ref handler, HasName, "-embed");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoFile))
                    AppendQueryParameter(ref handler, HasName, "-file");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoImage))
                    AppendQueryParameter(ref handler, HasName, "-image");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoVideo))
                    AppendQueryParameter(ref handler, HasName, "-video");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoSound))
                    AppendQueryParameter(ref handler, HasName, "-sound");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoSticker))
                    AppendQueryParameter(ref handler, HasName, "-sticker");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoPoll))
                    AppendQueryParameter(ref handler, HasName, "-poll");
                if (has.HasFlag(GuildMessagesSearchHasOptions.NoSnapshot))
                    AppendQueryParameter(ref handler, HasName, "-snapshot");
            }

            if (paginationProperties.LinkHostnames is { } linkHostnames)
            {
                foreach (var linkHostname in linkHostnames)
                    AppendQueryParameter(ref handler, "link_hostname", Uri.EscapeDataString(linkHostname));
            }

            if (paginationProperties.EmbedProviders is { } embedProviders)
            {
                foreach (var embedProvider in embedProviders)
                    AppendQueryParameter(ref handler, "embed_provider", Uri.EscapeDataString(embedProvider));
            }

            if (paginationProperties.EmbedTypes is { } embedTypes)
            {
                const string EmbedTypeName = "embed_type";

                if (embedTypes.HasFlag(GuildMessagesSearchEmbedTypes.Image))
                    AppendQueryParameter(ref handler, EmbedTypeName, "image");
                if (embedTypes.HasFlag(GuildMessagesSearchEmbedTypes.Video))
                    AppendQueryParameter(ref handler, EmbedTypeName, "video");
                if (embedTypes.HasFlag(GuildMessagesSearchEmbedTypes.Gifv))
                    AppendQueryParameter(ref handler, EmbedTypeName, "gif");
                if (embedTypes.HasFlag(GuildMessagesSearchEmbedTypes.Sound))
                    AppendQueryParameter(ref handler, EmbedTypeName, "sound");
                if (embedTypes.HasFlag(GuildMessagesSearchEmbedTypes.Article))
                    AppendQueryParameter(ref handler, EmbedTypeName, "article");
            }

            if (paginationProperties.AttachmentExtensions is { } attachmentExtensions)
            {
                foreach (var attachmentExtension in attachmentExtensions)
                    AppendQueryParameter(ref handler, "attachment_extension", Uri.EscapeDataString(attachmentExtension));
            }

            if (paginationProperties.AttachmentFilenames is { } attachmentFilenames)
            {
                foreach (var attachmentFilename in attachmentFilenames)
                    AppendQueryParameter(ref handler, "attachment_filename", Uri.EscapeDataString(attachmentFilename));
            }

            if (paginationProperties.Pinned is { } pinned)
                AppendQueryParameterT(ref handler, "pinned", pinned);

            if (paginationProperties.CommandId is { } commandId)
                AppendQueryParameterT(ref handler, "command_id", commandId);

            if (paginationProperties.CommandName is { } commandName)
                AppendQueryParameter(ref handler, "command_name", Uri.EscapeDataString(commandName));

            if (paginationProperties.IncludeNsfw is { } includeNsfw)
                AppendQueryParameterT(ref handler, "include_nsfw", includeNsfw);

            if (paginationProperties.ChannelIds is { } channelIds)
            {
                foreach (var channelId in channelIds)
                    AppendQueryParameterT(ref handler, "channel_id", channelId);
            }

            return handler.ToStringAndClear();

            static void AppendQueryParameter(ref DefaultInterpolatedStringHandler handler, string name, string value)
            {
                handler.AppendLiteral(name);
                handler.AppendLiteral("=");
                handler.AppendFormatted(value);
                handler.AppendLiteral("&");
            }

            static void AppendQueryParameterT<T>(ref DefaultInterpolatedStringHandler handler, string name, T value)
            {
                handler.AppendLiteral(name);
                handler.AppendLiteral("=");
                handler.AppendFormatted(value);
                handler.AppendLiteral("&");
            }

            [DoesNotReturn]
            [StackTraceHidden]
            static string InvalidEnum(string propertyName)
            {
                throw new InvalidOperationException($"Invalid '{propertyName}' value provided.");
            }
        }
    }

    /// <summary>
    /// Retrieves a message from a channel by its ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to retrieve.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<RestMessage> GetMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    /// <summary>
    /// Sends a new message to the specified text channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel to send the message to.</param>
    /// <param name="message">The content and properties of the message to send.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), TypeNameOverride = "Message")]
    public async Task<RestMessage> SendMessageAsync(ulong channelId, MessageProperties message, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = message.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Crossposts a message from an announcement channel to subscribed channels.
    /// </summary>
    /// <param name="channelId">The ID of the announcement channel containing the message.</param>
    /// <param name="messageId">The ID of the message to crosspost.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(AnnouncementGuildChannel)], nameof(AnnouncementGuildChannel.Id))]
    [GenerateAlias([typeof(AnnouncementGuildThread)], nameof(AnnouncementGuildThread.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<RestMessage> CrosspostMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    /// <summary>
    /// Adds a reaction to a specific message.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to react to.</param>
    /// <param name="emoji">The emoji to use as the reaction.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task AddMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Removes the current user's reaction from a specific message.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to remove the reaction from.</param>
    /// <param name="emoji">The emoji to remove as a reaction.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteCurrentUserMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Removes a specific user's reaction from a message.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to remove the reaction from.</param>
    /// <param name="emoji">The emoji to remove as a reaction.</param>
    /// <param name="userId">The ID of the user whose reaction will be removed.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteUserMessageReactionAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/{userId}", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of users who reacted to a message with a specific emoji.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to retrieve reactions for.</param>
    /// <param name="emoji">The emoji to filter reactions by.</param>
    /// <param name="paginationProperties">Pagination options for fetching users, or <see langword="null"/> to use defaults.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), type.HasValue ? $"?type={(byte)type.GetValueOrDefault()}&" : "?"),
            new(channelId),
            properties);
    }

    /// <summary>
    /// Removes all reactions from a message.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to clear reactions from.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteAllMessageReactionsAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Removes all reactions of a specific emoji from a message.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to clear emoji reactions from.</param>
    /// <param name="emoji">The emoji to remove from all users.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteAllMessageReactionsForEmojiAsync(ulong channelId, ulong messageId, ReactionEmojiProperties emoji, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}", null, new(channelId), properties, cancellationToken: cancellationToken);

    private static string ReactionEmojiToString(ReactionEmojiProperties emoji)
    {
        var id = emoji.Id;
        return id.HasValue ? $"{emoji.Name}:{id.GetValueOrDefault()}" : Uri.EscapeDataString(emoji.Name);
    }

    /// <summary>
    /// Modifies the contents of a specific message.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to modify.</param>
    /// <param name="action">An action that sets the new message properties.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<RestMessage> ModifyMessageAsync(ulong channelId, ulong messageId, Action<MessageOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Deletes a message from a channel by its ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to delete.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task DeleteMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Deletes multiple messages in bulk or individually depending on batch size.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the messages.</param>
    /// <param name="messageIds">The list of message IDs to delete.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public async Task DeleteMessagesAsync(ulong channelId, IEnumerable<ulong> messageIds, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        var ids = ArrayPool<ulong>.Shared.Rent(100);
        try
        {
            int index = 0;
            foreach (var id in messageIds)
            {
                ids[index] = id;

                if (index is 99)
                {
                    await BulkDeleteMessagesAsync(channelId, new(ids, 0, 100), properties, cancellationToken).ConfigureAwait(false);
                    index = 0;
                }
                else
                    index++;
            }

            if (index > 1)
                await BulkDeleteMessagesAsync(channelId, new(ids, 0, index), properties, cancellationToken).ConfigureAwait(false);
            else if (index is 1)
                await DeleteMessageAsync(channelId, ids[0], properties, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<ulong>.Shared.Return(ids);
        }
    }

    private async Task BulkDeleteMessagesAsync(ulong channelId, ReadOnlyMemory<ulong> messageIds, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<BulkDeleteMessagesProperties>(new(messageIds), Serialization.Default.BulkDeleteMessagesProperties))
            await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages/bulk-delete", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Modifies the permissions of a role or user in a guild channel.
    /// </summary>
    /// <param name="channelId">The ID of the guild channel to update permissions for.</param>
    /// <param name="permissionOverwrite">The permission overwrite to apply.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public async Task ModifyGuildChannelPermissionsAsync(ulong channelId, PermissionOverwriteProperties permissionOverwrite, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<PermissionOverwriteProperties>(permissionOverwrite, Serialization.Default.PermissionOverwriteProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a list of active invites for a guild channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel to retrieve invites for.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public async Task<IEnumerable<RestInvite>> GetGuildChannelInvitesAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInviteArray).ConfigureAwait(false)).Select(r => new RestInvite(r, this));

    /// <summary>
    /// Creates a new invite for a guild channel.
    /// </summary>
    /// <param name="channelId">The ID of the guild channel to create the invite for.</param>
    /// <param name="inviteProperties">The properties to configure the new invite. Can be <see langword="null"/> for defaults.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public async Task<RestInvite> CreateGuildChannelInviteAsync(ulong channelId, InviteProperties? inviteProperties = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        if (inviteProperties is null)
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/invites", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);

        using (HttpContent content = inviteProperties.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/invites", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Deletes a permission overwrite for a user or role in a guild channel.
    /// </summary>
    /// <param name="channelId">The ID of the guild channel.</param>
    /// <param name="overwriteId">The ID of the role or user whose permission overwrite is to be deleted.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IGuildChannel)], nameof(IGuildChannel.Id))]
    public Task DeleteGuildChannelPermissionAsync(ulong channelId, ulong overwriteId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Follows an announcement channel to another channel.
    /// </summary>
    /// <param name="channelId">The ID of the source announcement channel.</param>
    /// <param name="webhookChannelId">The ID of the channel to receive crossposted messages.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(AnnouncementGuildChannel)], nameof(AnnouncementGuildChannel.Id))]
    [GenerateAlias([typeof(AnnouncementGuildThread)], nameof(AnnouncementGuildThread.Id), TypeNameOverride = nameof(AnnouncementGuildChannel))]
    public async Task<FollowedChannel> FollowAnnouncementGuildChannelAsync(ulong channelId, ulong webhookChannelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<FollowAnnouncementGuildChannelProperties>(new(webhookChannelId), Serialization.Default.FollowAnnouncementGuildChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/followers", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonFollowedChannel).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Sends a typing indicator to the channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel to trigger typing in.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public Task TriggerTypingAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Enters a persistent typing state for the current user.
    /// </summary>
    /// <param name="channelId">The ID of the channel to type in.</param>
    /// <param name="scopeProperties">Optional properties to customize the typing interval, can be <see langword="null"/>.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public ValueTask<IDisposable> EnterTypingScopeAsync(ulong channelId, TypingScopeProperties? scopeProperties = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        AsyncTypingScope scope = new(this, channelId, scopeProperties, properties, cancellationToken);

        return new ValueTask<IDisposable>(scope, scope.Version);
    }

    /// <summary>
    /// Enters a persistent typing state for the current user.
    /// </summary>
    /// <param name="channelId">The ID of the channel to type in.</param>
    /// <param name="scopeProperties">Optional properties to customize the typing interval, can be <see langword="null"/>.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public IDisposable EnterTypingScope(ulong channelId, TypingScopeProperties? scopeProperties = null, RestRequestProperties? properties = null)
    {
        return new TypingScope(this, channelId, scopeProperties, properties);
    }

    /// <summary>
    /// Retrieves all pinned messages in a channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel to get pinned messages from.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    public async Task<IReadOnlyList<RestMessage>> GetPinnedMessagesAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessageArray).ConfigureAwait(false)).Select(m => new RestMessage(m, this)).ToArray();

    /// <summary>
    /// Pins a message in a channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to pin.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task PinMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Unpins a message from a channel.
    /// </summary>
    /// <param name="channelId">The ID of the channel containing the message.</param>
    /// <param name="messageId">The ID of the message to unpin.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public Task UnpinMessageAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Adds a recipient to a group DM channel.
    /// </summary>
    /// <param name="channelId">The ID of the group DM channel.</param>
    /// <param name="userId">The ID of the user to add.</param>
    /// <param name="groupDMChannelUserAddProperties">Properties for adding the user, such as access tokens.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GroupDMChannel)], nameof(GroupDMChannel.Id))]
    public async Task GroupDMChannelAddUserAsync(ulong channelId, ulong userId, GroupDMChannelUserAddProperties groupDMChannelUserAddProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GroupDMChannelUserAddProperties>(groupDMChannelUserAddProperties, Serialization.Default.GroupDMChannelUserAddProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/channels/{channelId}/recipients/{userId}", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a recipient from a group DM channel.
    /// </summary>
    /// <param name="channelId">The ID of the group DM channel.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GroupDMChannel)], nameof(GroupDMChannel.Id))]
    public Task GroupDMChannelDeleteUserAsync(ulong channelId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", null, new(channelId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Creates a thread from an existing message in a text channel.
    /// </summary>
    /// <param name="channelId">The ID of the text channel containing the message.</param>
    /// <param name="messageId">The ID of the message to start the thread from.</param>
    /// <param name="threadFromMessageProperties">The properties of the thread to create.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    [GenerateAlias([typeof(RestMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = "Message")]
    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, ulong messageId, GuildThreadFromMessageProperties threadFromMessageProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildThreadFromMessageProperties>(threadFromMessageProperties, Serialization.Default.GuildThreadFromMessageProperties))
            return GuildThread.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/messages/{messageId}/threads", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Creates a standalone thread in a text channel.
    /// </summary>
    /// <param name="channelId">The ID of the text channel to create the thread in.</param>
    /// <param name="threadProperties">The properties of the thread to create.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    public async Task<GuildThread> CreateGuildThreadAsync(ulong channelId, GuildThreadProperties threadProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildThreadProperties>(threadProperties, Serialization.Default.GuildThreadProperties))
            return GuildThread.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/threads", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Creates a new thread in a forum channel.
    /// </summary>
    /// <param name="channelId">The ID of the forum channel.</param>
    /// <param name="threadProperties">The properties of the forum thread to create.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(ForumGuildChannel)], nameof(ForumGuildChannel.Id))]
    public async Task<ForumGuildThread> CreateForumGuildThreadAsync(ulong channelId, ForumGuildThreadProperties threadProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = threadProperties.Serialize())
            return new ForumGuildThread(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/threads", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Joins the current user to a thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread to join.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public Task JoinGuildThreadAsync(ulong threadId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/@me", null, new(threadId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Adds another user to a thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="userId">The ID of the user to add to the thread.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public Task AddGuildThreadUserAsync(ulong threadId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/{userId}", null, new(threadId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Removes the current user from a thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread to leave.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public Task LeaveGuildThreadAsync(ulong threadId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/@me", null, new(threadId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Removes a user from a thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="userId">The ID of the user to remove from the thread.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    [GenerateAlias([typeof(GuildThreadUser)], nameof(GuildThreadUser.ThreadId), nameof(GuildThreadUser.Id))]
    public Task DeleteGuildThreadUserAsync(ulong threadId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/{userId}", null, new(threadId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Retrieves a thread member object for a user in the thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="userId">The ID of the user to fetch thread membership info for.</param>
    /// <param name="withGuildUser">Whether to include full guild member info in the response.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(GuildThread)], nameof(GuildThread.Id))]
    public async Task<ThreadUser> GetGuildThreadUserAsync(ulong threadId, ulong userId, bool withGuildUser = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        var user = await (await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members/{userId}", $"?with_member={withGuildUser}", new(threadId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonThreadUser).ConfigureAwait(false);
        return withGuildUser ? new GuildThreadUser(user, this) : new ThreadUser(user, this);
    }

    /// <summary>
    /// Retrieves a list of users participating in a thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="paginationProperties">Pagination options for fetching users, or <see langword="null"/> to use defaults.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), $"?with_member={withGuildUsers}&"),
            new(threadId),
            properties);
    }

    /// <summary>
    /// Retrieves public archived threads in the channel.
    /// </summary>
    /// <param name="channelId">The ID of the text channel.</param>
    /// <param name="paginationProperties">Pagination options for archived threads, or <see langword="null"/> to use defaults.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), t => t.ToString("s")),
            new(channelId),
            properties);
    }

    /// <summary>
    /// Retrieves private archived threads in the channel.
    /// </summary>
    /// <param name="channelId">The ID of the text channel.</param>
    /// <param name="paginationProperties">Pagination options for archived threads, or <see langword="null"/> to use defaults.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), t => t.ToString("s")),
            new(channelId),
            properties);
    }

    /// <summary>
    /// Retrieves private archived threads joined by the current user.
    /// </summary>
    /// <param name="channelId">The ID of the text channel.</param>
    /// <param name="paginationProperties">Pagination options for archived threads, or <see langword="null"/> to use defaults.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }
}
