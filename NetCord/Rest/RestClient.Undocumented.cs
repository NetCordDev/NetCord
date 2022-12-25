namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetApplicationAsync(ulong applicationId, RequestProperties? properties = null) => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/rpc", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication).ConfigureAwait(false), this);

    public async Task<IEnumerable<GoogleCloudPlatformStorageBucket>> CreateGoogleCloudPlatformStorageBucketAsync(ulong channelId, IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GoogleCloudPlatformStorageBucketsProperties>(new(buckets), GoogleCloudPlatformStorageBucketsProperties.GoogleCloudPlatformStorageBucketsPropertiesSerializerContext.WithOptions.GoogleCloudPlatformStorageBucketsProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/attachments", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonCreateGoogleCloudPlatformStorageBucketResult.JsonCreateGoogleCloudPlatformStorageBucketResultSerializerContext.WithOptions.JsonCreateGoogleCloudPlatformStorageBucketResult).ConfigureAwait(false)).Buckets.Select(a => new GoogleCloudPlatformStorageBucket(a));
    }
}
