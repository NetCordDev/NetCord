using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildTemplate), properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);

    public async Task<RestGuild> CreateGuildFromGuildTemplateAsync(string templateCode, GuildFromGuildTemplateProperties guildProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildFromGuildTemplateProperties>(guildProperties, GuildFromGuildTemplateProperties.GuildFromGuildTemplatePropertiesSerializerContext.WithOptions.GuildFromGuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/templates/{templateCode}", new(RateLimits.RouteParameter.CreateGuildFromGuildTemplate), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild).ConfigureAwait(false), this);
    }

    public async Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/templates", properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildTemplate.JsonGuildTemplateArraySerializerContext.WithOptions.JsonGuildTemplateArray).ConfigureAwait(false)).Select(t => new GuildTemplate(t, this));

    public async Task<GuildTemplate> CreateGuildTemplateAsync(ulong guildId, GuildTemplateProperties guildTemplateProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildTemplateProperties>(guildTemplateProperties, GuildTemplateProperties.GuildTemplatePropertiesSerializerContext.WithOptions.GuildTemplateProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/templates", new(RateLimits.RouteParameter.CreateSyncGuildTemplate), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    public async Task<GuildTemplate> SyncGuildTemplateAsync(ulong guildId, string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.CreateSyncGuildTemplate), properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);

    public async Task<GuildTemplate> ModifyGuildTemplateAsync(ulong guildId, string templateCode, Action<GuildTemplateOptions> action, RequestProperties? properties = null)
    {
        GuildTemplateOptions guildTemplateOptions = new();
        action(guildTemplateOptions);
        using (HttpContent content = new JsonContent<GuildTemplateOptions>(guildTemplateOptions, GuildTemplateOptions.GuildTemplateOptionsSerializerContext.WithOptions.GuildTemplateOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/templates/{templateCode}", new(RateLimits.RouteParameter.ModifyGuildTemplate), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);
    }

    public async Task<GuildTemplate> DeleteGuildTemplateAsync(ulong guildId, string templateCode, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/templates/{templateCode}", new RateLimits.Route(RateLimits.RouteParameter.DeleteGuildTemplate), properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildTemplate.JsonGuildTemplateSerializerContext.WithOptions.JsonGuildTemplate).ConfigureAwait(false), this);
}
