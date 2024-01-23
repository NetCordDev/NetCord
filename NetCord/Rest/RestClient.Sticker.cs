using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StandardSticker> GetStickerAsync(ulong stickerId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/stickers/{stickerId}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false));

    public async Task<IReadOnlyDictionary<ulong, StickerPack>> GetStickerPacksAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/sticker-packs", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStickerPacks).ConfigureAwait(false)).StickerPacks.ToDictionary(s => s.Id, s => new StickerPack(s));

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, GuildSticker>> GetGuildStickersAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStickerArray).ConfigureAwait(false)).ToDictionary(s => s.Id, s => new GuildSticker(s, this));

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildSticker), nameof(GuildSticker.GuildId), nameof(GuildSticker.Id))]
    public async Task<GuildSticker> GetGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false), this);

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildSticker> CreateGuildStickerAsync(ulong guildId, GuildStickerProperties sticker, RequestProperties? properties = null)
    {
        using (HttpContent content = sticker.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/stickers", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false), this);
    }

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildSticker), nameof(GuildSticker.GuildId), nameof(GuildSticker.Id))]
    public async Task<GuildSticker> ModifyGuildStickerAsync(ulong guildId, ulong stickerId, Action<GuildStickerOptions> action, RequestProperties? properties = null)
    {
        GuildStickerOptions guildStickerOptions = new();
        action(guildStickerOptions);
        using (HttpContent content = new JsonContent<GuildStickerOptions>(guildStickerOptions, Serialization.Default.GuildStickerOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSticker).ConfigureAwait(false), this);
    }

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildSticker), nameof(GuildSticker.GuildId), nameof(GuildSticker.Id))]
    public Task DeleteGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties);
}
