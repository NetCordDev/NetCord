namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetApplicationAsync(ulong applicationId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/rpc", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);

    public async Task<IEnumerable<GoogleCloudPlatformStorageBucket>> CreateGoogleCloudPlatformStorageBucketAsync(ulong channelId, IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GoogleCloudPlatformStorageBucketsProperties>(new(buckets), Serialization.Default.GoogleCloudPlatformStorageBucketsProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/attachments", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonCreateGoogleCloudPlatformStorageBucketResult).ConfigureAwait(false)).Buckets.Select(a => new GoogleCloudPlatformStorageBucket(a));
    }

    public IAsyncEnumerable<GuildUserInfo> SearchGuildUsersAsync(ulong guildId, GuildUsersSearchPaginationProperties? paginationProperties = null, RequestProperties? properties = null)
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
