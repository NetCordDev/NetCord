using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/voice/regions", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<VoiceState> GetCurrentGuildUserVoiceStateAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/voice-states/@me", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonVoiceState).ConfigureAwait(false), guildId, this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public async Task<VoiceState> GetGuildUserVoiceStateAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/voice-states/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonVoiceState).ConfigureAwait(false), guildId, this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task ModifyCurrentGuildUserVoiceStateAsync(ulong guildId, Action<CurrentUserVoiceStateOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        CurrentUserVoiceStateOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<CurrentUserVoiceStateOptions>(obj, Serialization.Default.CurrentUserVoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/voice-states/@me", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), null, nameof(GuildUser.Id))]
    public async Task ModifyGuildUserVoiceStateAsync(ulong guildId, ulong channelId, ulong userId, Action<VoiceStateOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        VoiceStateOptions obj = new(channelId);
        action(obj);
        using (HttpContent content = new JsonContent<VoiceStateOptions>(obj, Serialization.Default.VoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/voice-states/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
