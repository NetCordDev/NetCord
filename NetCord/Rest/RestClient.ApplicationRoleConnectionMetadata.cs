namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<ApplicationRoleConnectionMetadata>> GetApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, RestRequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/role-connections/metadata", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m));

    public async Task<IEnumerable<ApplicationRoleConnectionMetadata>> UpdateApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, IEnumerable<ApplicationRoleConnectionMetadataProperties> applicationRoleConnectionMetadataProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationRoleConnectionMetadataProperties>>(applicationRoleConnectionMetadataProperties, Serialization.Default.IEnumerableApplicationRoleConnectionMetadataProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/role-connections/metadata", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m));
    }
}
