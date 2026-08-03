namespace NetCord.Rest;

public partial class RestClient
{
    /// <summary>
    /// Returns a list of <see cref="ApplicationRoleConnectionMetadata"/> objects for the given application.
    /// </summary>
    /// <param name="applicationId">The ID of the application to request data for.</param>
    /// <param name="properties">Optional properties to customize the REST request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
    [GenerateAlias([typeof(CurrentApplication)], nameof(CurrentApplication.Id), TypeNameOverride = nameof(Application))]
    public async Task<IReadOnlyList<ApplicationRoleConnectionMetadata>> GetApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/role-connections/metadata", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m)).ToArray();

    /// <summary>
    /// Updates an application's <see cref="ApplicationRoleConnectionMetadata"/> list, replacing it with the given list instead.
    /// </summary>
    /// <param name="applicationId">The ID of the application to modify data for.</param>
    /// <param name="applicationRoleConnectionMetadataProperties">The list to overwrite the target data with.</param>
    /// <param name="properties">Optional properties to customize the REST request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
    [GenerateAlias([typeof(CurrentApplication)], nameof(CurrentApplication.Id), TypeNameOverride = nameof(Application))]
    public async Task<IReadOnlyList<ApplicationRoleConnectionMetadata>> UpdateApplicationRoleConnectionMetadataRecordsAsync(ulong applicationId, IEnumerable<ApplicationRoleConnectionMetadataProperties> applicationRoleConnectionMetadataProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationRoleConnectionMetadataProperties>>(applicationRoleConnectionMetadataProperties, Serialization.Default.IEnumerableApplicationRoleConnectionMetadataProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, content, $"/applications/{applicationId}/role-connections/metadata", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.IEnumerableJsonApplicationRoleConnectionMetadata).ConfigureAwait(false)).Select(m => new ApplicationRoleConnectionMetadata(m)).ToArray();
    }
}
