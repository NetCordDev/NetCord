using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/templates/{templateCode}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);

    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.Code), NameOverride = "CreateGuildAsync")]
    public async Task<RestGuild> CreateGuildFromGuildTemplateAsync(string templateCode, GuildFromGuildTemplateProperties guildProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildFromGuildTemplateProperties>(guildProperties, Serialization.Default.GuildFromGuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/templates/{templateCode}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(ulong guildId, RestRequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/templates", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplateArray).ConfigureAwait(false)).Select(t => new GuildTemplate(t, this));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildTemplate> CreateGuildTemplateAsync(ulong guildId, GuildTemplateProperties guildTemplateProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildTemplateProperties>(guildTemplateProperties, Serialization.Default.GuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/templates", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.SourceGuildId), nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> SyncGuildTemplateAsync(ulong guildId, string templateCode, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.SourceGuildId), nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> ModifyGuildTemplateAsync(ulong guildId, string templateCode, Action<GuildTemplateOptions> action, RestRequestProperties? properties = null)
    {
        GuildTemplateOptions guildTemplateOptions = new();
        action(guildTemplateOptions);
        using (HttpContent content = new JsonContent<GuildTemplateOptions>(guildTemplateOptions, Serialization.Default.GuildTemplateOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildTemplate)], nameof(GuildTemplate.SourceGuildId), nameof(GuildTemplate.Code))]
    public async Task<GuildTemplate> DeleteGuildTemplateAsync(ulong guildId, string templateCode, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
}
