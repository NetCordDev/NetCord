using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(Application)], nameof(Application.Id), Modifiers = ["virtual"])]
    public async Task<Application> GetApplicationAsync(ulong applicationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/rpc", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);

    [GenerateAlias([typeof(Channel)], nameof(Channel.Id))]
    public async Task<IReadOnlyList<GoogleCloudPlatformStorageBucket>> CreateGoogleCloudPlatformStorageBucketsAsync(ulong channelId, IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GoogleCloudPlatformStorageBucketsProperties>(new(buckets), Serialization.Default.GoogleCloudPlatformStorageBucketsProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/attachments", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonCreateGoogleCloudPlatformStorageBucketResult).ConfigureAwait(false)).Buckets.Select(a => new GoogleCloudPlatformStorageBucket(a)).ToArray();
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public IAsyncEnumerable<GuildUserInfo> SearchGuildUsersAsync(ulong guildId, GuildUsersSearchPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<GuildUsersSearchTimestamp>.Prepare(paginationProperties, GuildUsersSearchTimestamp.MinValue, GuildUsersSearchTimestamp.MaxValue, PaginationDirection.Before, 1000);

        return new ContentPaginationAsyncEnumerable<GuildUserInfo, GuildUsersSearchPaginationProperties, GuildUsersSearchTimestamp>(
            this,
            paginationProperties,
            paginationProperties.Direction.GetValueOrDefault() switch
            {
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildUsersSearchResult).ConfigureAwait(false)).Users.Select(i => new GuildUserInfo(i, guildId, this)),
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildUsersSearchResult).ConfigureAwait(false)).Users.GetReversedIEnumerable().Select(i => new GuildUserInfo(i, guildId, this)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            i =>
            {
                var user = i.User;
                return new(user.JoinedAt.GetValueOrDefault(), user.Id);
            },
            HttpMethod.Post,
            $"/guilds/{guildId}/members-search",
            new(paginationProperties, Serialization.Default.GuildUsersSearchPaginationProperties),
            new(guildId),
            properties);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public IAsyncEnumerable<GuildMessageSearchResult> SearchGuildMessagesAsync(ulong guildId, GuildMessagesSearchPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = GuildMessagesSearchPaginationProperties.PrepareWithOffset(paginationProperties, 0, PaginationDirection.After, 25);

        return new RetryingLimitedOffsetQueryPaginationAsyncEnumerable<GuildMessageSearchResult, int>(
            this,
            paginationProperties,
            9975,
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
}
