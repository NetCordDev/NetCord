namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetApplicationAsync(ulong applicationId, RequestProperties? properties = null) => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/rpc", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication), this);

    public async Task<IEnumerable<GoogleCloudPlatformStorageBucket>> CreateGoogleCloudPlatformStorageBucketAsync(ulong channelId, IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/attachments", new JsonContent<GoogleCloudPlatformStorageBucketsProperties>(new(buckets), GoogleCloudPlatformStorageBucketsProperties.GoogleCloudPlatformStorageBucketsPropertiesSerializerContext.WithOptions.GoogleCloudPlatformStorageBucketsProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonCreateGoogleCloudPlatformStorageBucketResult.JsonCreateGoogleCloudPlatformStorageBucketResultSerializerContext.WithOptions.JsonCreateGoogleCloudPlatformStorageBucketResult).Buckets.Select(a => new GoogleCloudPlatformStorageBucket(a));
}
