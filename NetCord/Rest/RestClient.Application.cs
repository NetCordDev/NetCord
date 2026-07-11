namespace NetCord.Rest;

public partial class RestClient
{
    /// <summary>
    /// Retrieves the application associated with the current user.
    /// </summary>
    /// <param name="properties">Optional properties to customize the REST request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
    [GenerateAlias([typeof(CurrentApplication)], CastType = typeof(Application), Modifiers = ["override"])]
    public async Task<CurrentApplication> GetCurrentApplicationAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/@me", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);

    /// <summary>
    /// Modifies the application associated with the current user.
    /// </summary>
    /// <param name="action">An action representing the modification to be made.</param>
    /// <param name="properties">Optional properties to customize the REST request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
    [GenerateAlias([typeof(CurrentApplication)])]
    public async Task<CurrentApplication> ModifyCurrentApplicationAsync(Action<CurrentApplicationOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        CurrentApplicationOptions currentApplicationOptions = new();
        action(currentApplicationOptions);
        using (HttpContent content = new JsonContent<CurrentApplicationOptions>(currentApplicationOptions, Serialization.Default.CurrentApplicationOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/applications/@me", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);
    }
}
