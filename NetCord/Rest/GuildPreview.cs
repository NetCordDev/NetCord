using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildPreview(JsonGuildPreview jsonModel, RestClient client) : ClientEntity(jsonModel, client)
{
    public string Name { get; } = jsonModel.Name;

    public string? IconHash { get; } = jsonModel.IconHash;

    public string? SplashHash { get; } = jsonModel.SplashHash;

    public string? DiscoverySplashHash { get; } = jsonModel.DiscoverySplashHash;

    public IReadOnlyDictionary<ulong, GuildEmoji> Emojis { get; } = CreateEmojis(jsonModel, client);

    private static Dictionary<ulong, GuildEmoji> CreateEmojis(JsonGuildPreview jsonModel, RestClient client)
    {
        var guildId = jsonModel.Id;
        return jsonModel.Emojis.ToDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, client));
    }

    public IReadOnlyList<string> Features { get; } = jsonModel.Features;

    public int ApproximateUserCount { get; } = jsonModel.ApproximateUserCount;

    public int ApproximatePresenceCount { get; } = jsonModel.ApproximatePresenceCount;

    public string? Description { get; } = jsonModel.Description;

    public IReadOnlyDictionary<ulong, GuildSticker> Stickers { get; } = jsonModel.Stickers.ToDictionary(s => s.Id, s => new GuildSticker(s, client));
}
