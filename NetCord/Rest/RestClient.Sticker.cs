using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StandardSticker> GetStickerAsync(ulong stickerId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/stickers/{stickerId}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false));

    public async Task<IReadOnlyDictionary<ulong, StickerPack>> GetStickerPacksAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/sticker-packs", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonStickerPacks.JsonStickerPacksSerializerContext.WithOptions.JsonStickerPacks).ConfigureAwait(false)).StickerPacks.ToDictionary(s => s.Id, s => new StickerPack(s));

    public async Task<IReadOnlyDictionary<ulong, GuildSticker>> GetGuildStickersAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(JsonSticker.JsonStickerArraySerializerContext.WithOptions.JsonStickerArray).ConfigureAwait(false)).ToDictionary(s => s.Id, s => new GuildSticker(s, this));

    public async Task<GuildSticker> GetGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false), this);

    public async Task<GuildSticker> CreateGuildStickerAsync(ulong guildId, GuildStickerProperties sticker, RequestProperties? properties = null)
    {
        using (HttpContent content = sticker.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/stickers", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false), this);
    }

    public async Task<GuildSticker> ModifyGuildStickerAsync(ulong guildId, ulong stickerId, Action<GuildStickerOptions> action, RequestProperties? properties = null)
    {
        GuildStickerOptions guildStickerOptions = new();
        action(guildStickerOptions);
        using (HttpContent content = new JsonContent<GuildStickerOptions>(guildStickerOptions, GuildStickerOptions.GuildStickerOptionsSerializerContext.WithOptions.GuildStickerOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false), this);
    }

    public Task DeleteGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", null, new(guildId), properties);
}
