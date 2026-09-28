using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    /// <summary>
    /// Gets a guild template.
    /// </summary>
    /// <param name="templateCode">The code of the guild template to get.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The guild template.</returns>
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/templates/{templateCode}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);

    /// <summary>
    /// Creates a guild from a guild template.
    /// </summary>
    /// <remarks>
    /// Discord deprecated application-driven guild creation in April 2025
    /// and removed the corresponding API endpoint in July 2025.
    /// </remarks>
    /// <param name="templateCode">The code of the guild template to use.</param>
    /// <param name="guildProperties">The properties of the guild to create.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The created guild.</returns>
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.Code), NameOverride = "CreateGuildAsync")]
    [Obsolete("Discord deprecated application-driven guild creation in April 2025 and removed the corresponding API endpoint in July 2025.")]
    public async Task<RestGuild> CreateGuildFromGuildTemplateAsync(string templateCode, GuildFromGuildTemplateProperties guildProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildFromGuildTemplateProperties>(guildProperties, Serialization.Default.GuildFromGuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/templates/{templateCode}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Gets the guild templates of a guild.
    /// </summary>
    /// <remarks>
    /// Requires the <c>MANAGE_GUILD</c> permission.
    /// </remarks>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The guild templates.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/templates", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplateArray).ConfigureAwait(false)).Select(t => new GuildTemplate(t, this));

    /// <summary>
    /// Creates a guild template.
    /// </summary>
    /// <remarks>
    /// Requires the <c>MANAGE_GUILD</c> permission.
    /// </remarks>
    /// <param name="guildId">The ID of the guild to create the template for.</param>
    /// <param name="guildTemplateProperties">The properties of the guild template.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The created guild template.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildTemplate> CreateGuildTemplateAsync(ulong guildId, GuildTemplateProperties guildTemplateProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildTemplateProperties>(guildTemplateProperties, Serialization.Default.GuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/templates", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Syncs a guild template.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="templateCode">The code of the template.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The synced guild template.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.SourceGuildId), nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> SyncGuildTemplateAsync(ulong guildId, string templateCode, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);

    /// <summary>
    /// Modifies a guild template.
    /// </summary>
    /// <remarks>
    /// Requires the <c>MANAGE_GUILD</c> permission.
    /// </remarks>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="templateCode">The code of the template.</param>
    /// <param name="action">The action to perform on the guild template options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified guild template.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.SourceGuildId), nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> ModifyGuildTemplateAsync(ulong guildId, string templateCode, Action<GuildTemplateOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildTemplateOptions guildTemplateOptions = new();
        action(guildTemplateOptions);
        using (HttpContent content = new JsonContent<GuildTemplateOptions>(guildTemplateOptions, Serialization.Default.GuildTemplateOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Deletes a guild template.
    /// </summary>
    /// <remarks>
    /// Requires the <c>MANAGE_GUILD</c> permission.
    /// </remarks>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="templateCode">The code of the template.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The deleted guild template.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.SourceGuildId), nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> DeleteGuildTemplateAsync(ulong guildId, string templateCode, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
}
