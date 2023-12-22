namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/templates/{templateCode}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);

    public async Task<RestGuild> CreateGuildFromGuildTemplateAsync(string templateCode, GuildFromGuildTemplateProperties guildProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildFromGuildTemplateProperties>(guildProperties, Serialization.Default.GuildFromGuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/templates/{templateCode}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    public async Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/templates", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplateArray).ConfigureAwait(false)).Select(t => new GuildTemplate(t, this));

    public async Task<GuildTemplate> CreateGuildTemplateAsync(ulong guildId, GuildTemplateProperties guildTemplateProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildTemplateProperties>(guildTemplateProperties, Serialization.Default.GuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/templates", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    public async Task<GuildTemplate> SyncGuildTemplateAsync(ulong guildId, string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);

    public async Task<GuildTemplate> ModifyGuildTemplateAsync(ulong guildId, string templateCode, Action<GuildTemplateOptions> action, RequestProperties? properties = null)
    {
        GuildTemplateOptions guildTemplateOptions = new();
        action(guildTemplateOptions);
        using (HttpContent content = new JsonContent<GuildTemplateOptions>(guildTemplateOptions, Serialization.Default.GuildTemplateOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    public async Task<GuildTemplate> DeleteGuildTemplateAsync(ulong guildId, string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/templates/{templateCode}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildTemplate).ConfigureAwait(false), this);
}
