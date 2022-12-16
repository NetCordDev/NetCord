namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<ApplicationRoleConnectionMetadata>> GetApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/role-connections/metadata", new RateLimits.Route(RateLimits.RouteParameter.GetApplicationRoleConnectionMetadataRecords), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonApplicationRoleConnectionMetadata.IEnumerableOfJsonApplicationRoleConnectionMetadataSerializerContext.WithOptions.IEnumerableJsonApplicationRoleConnectionMetadata).Select(m => new ApplicationRoleConnectionMetadata(m));

    public async Task<IEnumerable<ApplicationRoleConnectionMetadata>> UpdateApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, IEnumerable<ApplicationRoleConnectionMetadataProperties> applicationRoleConnectionMetadataProperties, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/role-connections/metadata", new(RateLimits.RouteParameter.UpdateApplicationRoleConnectionMetadataRecords), new JsonContent<IEnumerable<ApplicationRoleConnectionMetadataProperties>>(applicationRoleConnectionMetadataProperties, ApplicationRoleConnectionMetadataProperties.IEnumerableOfApplicationRoleConnectionMetadataPropertiesSerializerContext.WithOptions.IEnumerableApplicationRoleConnectionMetadataProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonApplicationRoleConnectionMetadata.IEnumerableOfJsonApplicationRoleConnectionMetadataSerializerContext.WithOptions.IEnumerableJsonApplicationRoleConnectionMetadata).Select(m => new ApplicationRoleConnectionMetadata(m));
}
