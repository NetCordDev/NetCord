using System.Text.Json;

namespace NetCord;

public partial class RestClient
{
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildProperties), "/guilds", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<RestGuild> GetGuildAsync(DiscordId guildId, bool withCounts = false, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}?with_counts={withCounts}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<GuildPreview> GetGuildPreviewAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<RestGuild> ModifyGuildAsync(DiscordId guildId, Action<GuildOptions> action, RequestProperties? options = null)
    {
        GuildOptions guildProperties = new();
        action(guildProperties);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildProperties), $"/guilds/{guildId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);
    }

    public Task DeleteGuildAsync(DiscordId guildId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", options);

    public async Task<IReadOnlyDictionary<DiscordId, GuildBan>> GetGuildBansAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan[]>().ToDictionary(b => b.User.Id, b => new GuildBan(b, this));

    public async Task<GuildBan> GetGuildBanAsync(DiscordId guildId, DiscordId userId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan>(), this);

    public async Task<IReadOnlyDictionary<DiscordId, GuildRole>> GetGuildRolesAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, this));

    public async Task<GuildRole> CreateGuildRoleAsync(DiscordId guildId, GuildRoleProperties guildRoleProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildRoleProperties), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), this);

    public async Task<IReadOnlyDictionary<DiscordId, GuildRole>> ModifyGuildRolePositionsAsync(DiscordId guildId, GuildRolePosition[] positions, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Patch, new JsonContent(positions), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, this));

    public async Task<GuildRole> ModifyGuildRoleAsync(DiscordId guildId, DiscordId roleId, Action<GuildRoleOptions> action, RequestProperties? options = null)
    {
        GuildRoleOptions obj = new();
        action(obj);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/roles/{roleId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), this);
    }

    public Task DeleteGuildRoleAsync(DiscordId guildId, DiscordId roleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", options);

    public async Task<int> GetGuildPruneCountAsync(DiscordId guildId, int days, DiscordId[]? roles = null, RequestProperties? options = null)
    {
        if (roles == null)
            return JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
        else
            return JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}&include_roles={string.Join(',', roles)}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
    }

    public async Task<int?> GuildPruneAsync(DiscordId guildId, GuildPruneProperties pruneProperties, RequestProperties? options = null)
        => JsonDocument.Parse(await SendRequestAsync(HttpMethod.Post, new JsonContent(pruneProperties), $"/guilds/{guildId}/prune", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").ToObject<int?>();

    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonVoiceRegion>>().Select(r => new VoiceRegion(r));

    public async Task<IEnumerable<RestGuildInvite>> GetGuildInvitesAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonRestGuildInvite>>().Select(i => new RestGuildInvite(i, this));

    public async Task<IReadOnlyDictionary<DiscordId, Integration>> GetGuildIntegrationsAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", options).ConfigureAwait(false))!.ToObject<IEnumerable<JsonModels.JsonIntegration>>().ToDictionary(i => i.Id, i => new Integration(i, this));

    public Task DeleteGuildIntegrationAsync(DiscordId guildId, DiscordId integrationId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", options);

    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());

    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(DiscordId guildId, Action<GuildWidgetSettingsOptions> action, RequestProperties? options = null)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWidgetSettingsOptions), $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());
    }

    public async Task<GuildWidget> GetGuildWidgetAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidget>(), this);

    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildVanityInvite>());

    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());

    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(DiscordId guildId, Action<GuildWelcomeScreenOptions> action, RequestProperties? options = null)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWelcomeScreenOptions), $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());
    }
}