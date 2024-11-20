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
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildUserSearchResult).ConfigureAwait(false)).Users.Select(i => new GuildUserInfo(i, guildId, this)),
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildUserSearchResult).ConfigureAwait(false)).Users.GetReversedIEnumerable().Select(i => new GuildUserInfo(i, guildId, this)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            i =>
            {
                var user = i.User;
                return new(user.JoinedAt, user.Id);
            },
            HttpMethod.Post,
            $"/guilds/{guildId}/members-search",
            new(paginationProperties, Serialization.Default.GuildUsersSearchPaginationProperties),
            new(guildId),
            properties);
    }
}
