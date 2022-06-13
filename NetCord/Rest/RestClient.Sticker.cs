namespace NetCord;

public partial class RestClient
{
    public async Task<StandardSticker> GetStickerAsync(Snowflake stickerId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/stickers/{stickerId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonSticker>());

    public async Task<IReadOnlyDictionary<Snowflake, StickerPack>> GetNitroStickerPacksAsync(RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/sticker-packs", options).ConfigureAwait(false)).ToObject<JsonModels.JsonStickerPack[]>().ToDictionary(s => s.Id, s => new StickerPack(s));

    public async Task<IReadOnlyDictionary<Snowflake, GuildSticker>> GetGuildStickersAsync(Snowflake guildId, RequestProperties? options = null)
    => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers", options).ConfigureAwait(false)).ToObject<JsonModels.JsonSticker[]>().ToDictionary(s => s.Id, s => new GuildSticker(s, this));

    public async Task<GuildSticker> GetGuildStickerAsync(Snowflake guildId, Snowflake stickerId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/stickers/{stickerId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonSticker>(), this);

    public async Task<GuildSticker> CreateGuildStickerAsync(Snowflake guildId, GuildStickerProperties sticker, RequestProperties? options = null)
    => new((await SendRequestAsync(HttpMethod.Post, sticker.Build(), $"/guilds/{guildId}/stickers", options).ConfigureAwait(false)).ToObject<JsonModels.JsonSticker>(), this);

    public async Task<GuildSticker> ModifyGuildStickerAsync(Snowflake guildId, Snowflake stickerId, Action<GuildStickerOptions> action, RequestProperties? options = null)
    {
        GuildStickerOptions guildStickerOptions = new();
        action(guildStickerOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildStickerOptions), $"/guilds/{guildId}/stickers/{stickerId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonSticker>(), this);
    }

    public Task DeleteGuildStickerAsync(Snowflake guildId, Snowflake stickerId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", options);
}