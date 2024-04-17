using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias(typeof(Application), nameof(Application.Id), Modifiers = ["virtual"])]
    public async Task<Application> GetApplicationAsync(ulong applicationId, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/rpc", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);

    [GenerateAlias(typeof(Channel), nameof(Channel.Id))]
    public async Task<IEnumerable<GoogleCloudPlatformStorageBucket>> CreateGoogleCloudPlatformStorageBucketsAsync(ulong channelId, IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GoogleCloudPlatformStorageBucketsProperties>(new(buckets), Serialization.Default.GoogleCloudPlatformStorageBucketsProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/attachments", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonCreateGoogleCloudPlatformStorageBucketResult).ConfigureAwait(false)).Buckets.Select(a => new GoogleCloudPlatformStorageBucket(a));
    }

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
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
    
    [GenerateAlias(typeof(RestMessage), nameof(RestMessage.Id))]
    [GenerateAlias(typeof(TextChannel), nameof(TextChannel.Id))]
    public async Task<RestMessage> ExpirePollAsync(ulong messageId, ulong channelId, RestRequestProperties? properties = null)
    {
        var stream = await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/polls/{messageId}/expire").ConfigureAwait(false);
        
        return new(await stream.ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }
}
