namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildTemplate), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate), this);

    public async Task<RestGuild> CreateGuildFromGuildTemplateAsync(string templateCode, GuildFromGuildTemplateProperties guildProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/guilds/templates/{templateCode}", new(RateLimits.RouteParameter.CreateGuildFromGuildTemplate), new JsonContent<GuildFromGuildTemplateProperties>(guildProperties, GuildFromGuildTemplateProperties.GuildFromGuildTemplatePropertiesSerializerContext.WithOptions.GuildFromGuildTemplateProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild), this);

    public async Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/templates", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildTemplate.JsonGuildTemplateArraySerializerContext.WithOptions.JsonGuildTemplateArray).Select(t => new GuildTemplate(t, this));

    public async Task<GuildTemplate> CreateGuildTemplateAsync(Snowflake guildId, GuildTemplateProperties guildTemplateProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/templates", new(RateLimits.RouteParameter.CreateSyncGuildTemplate), new JsonContent<GuildTemplateProperties>(guildTemplateProperties, GuildTemplateProperties.GuildTemplatePropertiesSerializerContext.WithOptions.GuildTemplateProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate), this);

    public async Task<GuildTemplate> SyncGuildTemplateAsync(Snowflake guildId, string templateCode, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.CreateSyncGuildTemplate), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate), this);

    public async Task<GuildTemplate> ModifyGuildTemplateAsync(Snowflake guildId, string templateCode, Action<GuildTemplateOptions> action, RequestProperties? properties = null)
    {
        GuildTemplateOptions guildTemplateOptions = new();
        action(guildTemplateOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/templates/{templateCode}", new(RateLimits.RouteParameter.ModifyGuildTemplate), new JsonContent<GuildTemplateOptions>(guildTemplateOptions, GuildTemplateOptions.GuildTemplateOptionsSerializerContext.WithOptions.GuildTemplateOptions), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate), this);
    }

    public async Task<GuildTemplate> DeleteGuildTemplateAsync(Snowflake guildId, string templateCode, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.DeleteGuildTemplate), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate), this);
}
