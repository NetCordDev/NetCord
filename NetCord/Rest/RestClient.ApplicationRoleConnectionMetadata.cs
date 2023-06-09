using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<ApplicationRoleConnectionMetadata>> GetApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/role-connections/metadata", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationRoleConnectionMetadata.IEnumerableOfJsonApplicationRoleConnectionMetadataSerializerContext.WithOptions.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m));

    public async Task<IEnumerable<ApplicationRoleConnectionMetadata>> UpdateApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, IEnumerable<ApplicationRoleConnectionMetadataProperties> applicationRoleConnectionMetadataProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationRoleConnectionMetadataProperties>>(applicationRoleConnectionMetadataProperties, ApplicationRoleConnectionMetadataProperties.IEnumerableOfApplicationRoleConnectionMetadataPropertiesSerializerContext.WithOptions.IEnumerableApplicationRoleConnectionMetadataProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/role-connections/metadata", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationRoleConnectionMetadata.IEnumerableOfJsonApplicationRoleConnectionMetadataSerializerContext.WithOptions.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m));
    }
}
