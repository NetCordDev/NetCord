namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StandardSticker> GetStickerAsync(ulong stickerId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/stickers/{stickerId}", new RateLimits.Route(RateLimits.RouteParameter.GetSticker), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker)); //

    public async Task<IReadOnlyDictionary<ulong, StickerPack>> GetNitroStickerPacksAsync(RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/sticker-packs", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonNitroStickerPacks.JsonNitroStickerPacksSerializerContext.WithOptions.JsonNitroStickerPacks).StickerPacks.ToDictionary(s => s.Id, s => new StickerPack(s));

    public async Task<IReadOnlyDictionary<ulong, GuildSticker>> GetGuildStickersAsync(ulong guildId, RequestProperties? properties = null)
    => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonSticker.JsonStickerArraySerializerContext.WithOptions.JsonStickerArray).ToDictionary(s => s.Id, s => new GuildSticker(s, this));

    public async Task<GuildSticker> GetGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers/{stickerId}", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker), this);

    public async Task<GuildSticker> CreateGuildStickerAsync(ulong guildId, GuildStickerProperties sticker, RequestProperties? properties = null)
    => new((await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/stickers", sticker.Build(), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker), this);

    public async Task<GuildSticker> ModifyGuildStickerAsync(ulong guildId, ulong stickerId, Action<GuildStickerOptions> action, RequestProperties? properties = null)
    {
        GuildStickerOptions guildStickerOptions = new();
        action(guildStickerOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/stickers/{stickerId}", new(RateLimits.RouteParameter.ModifyGuildSticker), new JsonContent<GuildStickerOptions>(guildStickerOptions, GuildStickerOptions.GuildStickerOptionsSerializerContext.WithOptions.GuildStickerOptions), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker), this);
    }

    public Task DeleteGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteGuildSticker), properties);
}
