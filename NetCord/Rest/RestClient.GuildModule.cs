namespace NetCord;

public partial class RestClient
{
    public partial class GuildModule
    {
        private readonly RestClient _client;

        public UserModule User { get; }
        public ChannelModule Channel { get; }

        internal GuildModule(RestClient client)
        {
            _client = client;
            User = new(client);
            Channel = new(client);
        }

        public async Task<RestGuild> CreateAsync(GuildProperties guildProperties, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(guildProperties), "/guilds", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), _client);

        public async Task<RestGuild> GetAsync(DiscordId guildId, bool withCounts = false, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}?with_counts={withCounts}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), _client);

        public async Task<GuildPreview> GetPreviewAsync(DiscordId guildId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), _client);

        public async Task<RestGuild> ModifyAsync(DiscordId guildId, Action<GuildOptions> action, RequestOptions? options = null)
        {
            GuildOptions guildProperties = new();
            action(guildProperties);
            return new((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(guildProperties), $"/guilds/{guildId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), _client);
        }

        public Task DeleteAsync(DiscordId guildId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", options);

        public async Task<IReadOnlyDictionary<DiscordId, GuildBan>> GetBansAsync(DiscordId guildId, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan[]>().ToDictionary(b => b.User.Id, b => new GuildBan(b, _client));

        public async Task<GuildBan> GetBanAsync(DiscordId guildId, DiscordId userId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan>(), _client);

        public async Task<IReadOnlyDictionary<DiscordId, GuildRole>> GetRolesAsync(DiscordId guildId, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, _client));

        public async Task<GuildRole> CreateRoleAsync(DiscordId guildId, GuildRoleProperties guildRoleProperties, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(guildRoleProperties), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), _client);

        public async Task<IReadOnlyDictionary<DiscordId, GuildRole>> ModifyRolePositionsAsync(DiscordId guildId, GuildRolePosition[] positions, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(positions), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, _client));

        public async Task<GuildRole> ModifyRoleAsync(DiscordId guildId, DiscordId roleId, Action<GuildRoleOptions> action, RequestOptions? options = null)
        {
            GuildRoleOptions obj = new();
            action(obj);
            return new((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/roles/{roleId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), _client);
        }

        public Task DeleteRoleAsync(DiscordId guildId, DiscordId roleId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", options);

        public async Task<int> GetPruneCountAsync(DiscordId guildId, int days, DiscordId[]? roles = null, RequestOptions? options = null)
        {
            if (roles == null)
                return (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
            else
                return (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}&include_roles={string.Join(',', roles)}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
        }

        public async Task<int?> PruneAsync(DiscordId guildId, GuildPruneProperties pruneProperties, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(pruneProperties), $"/guilds/{guildId}/prune", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").ToObject<int?>();

        public async Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(DiscordId guildId, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", options).ConfigureAwait(false))!.RootElement.EnumerateArray().Select(r => new VoiceRegion(r.ToObject<JsonModels.JsonVoiceRegion>()));

        public async Task<IEnumerable<GuildInvite>> GetInvitesAsync(DiscordId guildId, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", options).ConfigureAwait(false))!.RootElement.EnumerateArray().Select(r => new GuildInvite(r.ToObject<JsonModels.JsonGuildInvite>(), _client));

        public async Task<IReadOnlyDictionary<DiscordId, Integration>> GetIntegrationsAsync(DiscordId guildId, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonIntegration[]>().ToDictionary(i => i.Id, i => new Integration(i, _client));

        public Task DeleteIntegrationAsync(DiscordId guildId, DiscordId integrationId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", options);

        public async Task<GuildWidgetSettings> GetWidgetSettingsAsync(DiscordId guildId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());

        public async Task<GuildWidgetSettings> ModifyWidgetSettingsAsync(DiscordId guildId, Action<GuildWidgetSettingsOptions> action, RequestOptions? options = null)
        {
            GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
            action(guildWidgetSettingsOptions);
            return new((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWidgetSettingsOptions), $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());
        }

        public async Task<GuildWidget> GetWidgetAsync(DiscordId guildId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidget>(), _client);

        public async Task<GuildVanityInvite> GetVanityInviteAsync(DiscordId guildId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildVanityInvite>());

        public async Task<GuildWelcomeScreen> GetWelcomeScreenAsync(DiscordId guildId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());

        public async Task<GuildWelcomeScreen> ModifyWelcomeScreenAsync(DiscordId guildId, Action<GuildWelcomeScreenOptions> action, RequestOptions? options = null)
        {
            GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
            action(guildWelcomeScreenOptions);
            return new((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWelcomeScreenOptions), $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());
        }

        public Task ModifyCurrentUserVoiceStateAsync(DiscordId guildId, DiscordId channelId, Action<CurrentUserVoiceStateOptions> action, RequestOptions? options = null)
        {
            CurrentUserVoiceStateOptions obj = new(channelId);
            action(obj);
            return _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/voice-states/@me", options);
        }

        public Task ModifyUserVoiceStateAsync(DiscordId guildId, DiscordId channelId, Action<VoiceStateOptions> action, RequestOptions? options = null)
        {
            VoiceStateOptions obj = new(channelId);
            action(obj);
            return _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/voice-states/@me", options);
        }
    }
}