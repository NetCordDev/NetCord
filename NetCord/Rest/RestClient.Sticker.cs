using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StandardSticker> GetStickerAsync(ulong stickerId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/stickers/{stickerId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false));

    public async Task<IReadOnlyList<StickerPack>> GetStickerPacksAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/sticker-packs", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStickerPacks).ConfigureAwait(false)).StickerPacks.Select(s => new StickerPack(s)).ToArray();

    public async Task<StickerPack> GetStickerPackAsync(ulong stickerPackId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/sticker-packs/{stickerPackId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStickerPack).ConfigureAwait(false));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<GuildSticker>> GetGuildStickersAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStickerArray).ConfigureAwait(false)).Select(s => new GuildSticker(s, this)).ToArray();

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildSticker)], nameof(GuildSticker.GuildId), nameof(GuildSticker.Id))]
    public async Task<GuildSticker> GetGuildStickerAsync(ulong guildId, ulong stickerId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildSticker> CreateGuildStickerAsync(ulong guildId, GuildStickerProperties sticker, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = sticker.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/stickers", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildSticker)], nameof(GuildSticker.GuildId), nameof(GuildSticker.Id))]
    public async Task<GuildSticker> ModifyGuildStickerAsync(ulong guildId, ulong stickerId, Action<GuildStickerOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildStickerOptions guildStickerOptions = new();
        action(guildStickerOptions);
        using (HttpContent content = new JsonContent<GuildStickerOptions>(guildStickerOptions, Serialization.Default.GuildStickerOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildSticker)], nameof(GuildSticker.GuildId), nameof(GuildSticker.Id))]
    public Task DeleteGuildStickerAsync(ulong guildId, ulong stickerId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties, cancellationToken: cancellationToken);
}
