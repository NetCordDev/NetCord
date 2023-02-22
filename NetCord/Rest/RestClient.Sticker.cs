namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StandardSticker> GetStickerAsync(ulong stickerId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/stickers/{stickerId}", new RateLimits.Route(RateLimits.RouteParameter.GetSticker), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false));

    public async Task<IReadOnlyDictionary<ulong, StickerPack>> GetNitroStickerPacksAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/sticker-packs", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonNitroStickerPacks.JsonNitroStickerPacksSerializerContext.WithOptions.JsonNitroStickerPacks).ConfigureAwait(false)).StickerPacks.ToDictionary(s => s.Id, s => new StickerPack(s));

    public async Task<IReadOnlyDictionary<ulong, GuildSticker>> GetGuildStickersAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonSticker.JsonStickerArraySerializerContext.WithOptions.JsonStickerArray).ConfigureAwait(false)).ToDictionary(s => s.Id, s => new GuildSticker(s, this));

    public async Task<GuildSticker> GetGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers/{stickerId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false), this);

    public async Task<GuildSticker> CreateGuildStickerAsync(ulong guildId, GuildStickerProperties sticker, RequestProperties? properties = null)
    {
        using (HttpContent content = sticker.Build())
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/stickers", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false), this);
    }

    public async Task<GuildSticker> ModifyGuildStickerAsync(ulong guildId, ulong stickerId, Action<GuildStickerOptions> action, RequestProperties? properties = null)
    {
        GuildStickerOptions guildStickerOptions = new();
        action(guildStickerOptions);
        using (HttpContent content = new JsonContent<GuildStickerOptions>(guildStickerOptions, GuildStickerOptions.GuildStickerOptionsSerializerContext.WithOptions.GuildStickerOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/stickers/{stickerId}", new(RateLimits.RouteParameter.ModifyGuildSticker), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonSticker.JsonStickerSerializerContext.WithOptions.JsonSticker).ConfigureAwait(false), this);
    }

    public Task DeleteGuildStickerAsync(ulong guildId, ulong stickerId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteGuildSticker), properties);
}
