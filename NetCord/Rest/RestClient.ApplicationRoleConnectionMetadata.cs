namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(CurrentApplication)], nameof(CurrentApplication.Id), TypeNameOverride = nameof(Application))]
    public async Task<IReadOnlyList<ApplicationRoleConnectionMetadata>> GetApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/role-connections/metadata", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m)).ToArray();

    [GenerateAlias([typeof(CurrentApplication)], nameof(CurrentApplication.Id), TypeNameOverride = nameof(Application))]
    public async Task<IReadOnlyList<ApplicationRoleConnectionMetadata>> UpdateApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, IEnumerable<ApplicationRoleConnectionMetadataProperties> applicationRoleConnectionMetadataProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationRoleConnectionMetadataProperties>>(applicationRoleConnectionMetadataProperties, Serialization.Default.IEnumerableApplicationRoleConnectionMetadataProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, content, $"/applications/{applicationId}/role-connections/metadata", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m)).ToArray();
    }
}
