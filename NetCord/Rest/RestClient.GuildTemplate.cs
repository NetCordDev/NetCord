namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildTemplate), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);

    public async Task<RestGuild> CreateGuildFromGuildTemplateAsync(string templateCode, GuildFromGuildTemplateProperties guildProperties, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/templates/{templateCode}", new(RateLimits.RouteParameter.CreateGuildFromGuildTemplate), new JsonContent<GuildFromGuildTemplateProperties>(guildProperties, GuildFromGuildTemplateProperties.GuildFromGuildTemplatePropertiesSerializerContext.WithOptions.GuildFromGuildTemplateProperties), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild).ConfigureAwait(false), this);

    public async Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/templates", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildTemplate.JsonGuildTemplateArraySerializerContext.WithOptions.JsonGuildTemplateArray).ConfigureAwait(false)).Select(t => new GuildTemplate(t, this));

    public async Task<GuildTemplate> CreateGuildTemplateAsync(ulong guildId, GuildTemplateProperties guildTemplateProperties, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/templates", new(RateLimits.RouteParameter.CreateSyncGuildTemplate), new JsonContent<GuildTemplateProperties>(guildTemplateProperties, GuildTemplateProperties.GuildTemplatePropertiesSerializerContext.WithOptions.GuildTemplateProperties), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);

    public async Task<GuildTemplate> SyncGuildTemplateAsync(ulong guildId, string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.CreateSyncGuildTemplate), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);

    public async Task<GuildTemplate> ModifyGuildTemplateAsync(ulong guildId, string templateCode, Action<GuildTemplateOptions> action, RequestProperties? properties = null)
    {
        GuildTemplateOptions guildTemplateOptions = new();
        action(guildTemplateOptions);
        return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/templates/{templateCode}", new(RateLimits.RouteParameter.ModifyGuildTemplate), new JsonContent<GuildTemplateOptions>(guildTemplateOptions, GuildTemplateOptions.GuildTemplateOptionsSerializerContext.WithOptions.GuildTemplateOptions), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    public async Task<GuildTemplate> DeleteGuildTemplateAsync(ulong guildId, string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.DeleteGuildTemplate), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);
}
