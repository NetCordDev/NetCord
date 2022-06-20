namespace NetCord;

public partial class RestClient
{
    public async Task<GuildTemplate> GetGuildTemplateAsync(string templateCode, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/templates/{templateCode}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildTemplate>(), this);

    public async Task<RestGuild> CreateGuildFromGuildTemplateAsync(string templateCode, GuildFromGuildTemplateProperties guildProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildProperties), $"/guilds/templates/{templateCode}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuild>(), this);

    public async Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/templates", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildTemplate[]>().Select(t => new GuildTemplate(t, this));

    public async Task<GuildTemplate> CreateGuildTemplateAsync(Snowflake guildId, GuildTemplateProperties guildTemplateProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildTemplateProperties), $"/guilds/{guildId}/templates", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildTemplate>(), this);

    public async Task<GuildTemplate> SyncGuildTemplateAsync(Snowflake guildId, string templateCode, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/templates/{templateCode}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildTemplate>(), this);

    public async Task<GuildTemplate> ModifyGuildTemplateAsync(Snowflake guildId, string templateCode, Action<GuildTemplateOptions> action, RequestProperties? options = null)
    {
        GuildTemplateOptions guildTemplateOptions = new();
        action(guildTemplateOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildTemplateOptions), $"/guilds/{guildId}/templates/{templateCode}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildTemplate>(), this);
    }

    public async Task<GuildTemplate> DeleteGuildTemplateAsync(Snowflake guildId, string templateCode, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/templates/{templateCode}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildTemplate>(), this);
}