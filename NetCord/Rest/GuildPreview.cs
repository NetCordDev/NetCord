using System.Collections.Immutable;

namespace NetCord.Rest;

public class GuildPreview : ClientEntity, IJsonModel<NetCord.JsonModels.JsonGuild>
{
    NetCord.JsonModels.JsonGuild IJsonModel<NetCord.JsonModels.JsonGuild>.JsonModel => _jsonModel;
    private readonly NetCord.JsonModels.JsonGuild _jsonModel;

    public GuildPreview(NetCord.JsonModels.JsonGuild jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        Emojis = _jsonModel.Emojis.ToImmutableDictionaryOrEmpty(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, Id, client));
    }

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public string? IconHash => _jsonModel.IconHash;

    public string? SplashHash => _jsonModel.SplashHash;

    public string? DiscoverySplashHash => _jsonModel.DiscoverySplashHash;

    public ImmutableDictionary<ulong, GuildEmoji> Emojis { get; }

    public IReadOnlyList<string> Features => _jsonModel.Features;

    public int ApproximateUserCount => _jsonModel.ApproximateUserCount.GetValueOrDefault();

    public int ApproximatePresenceCount => _jsonModel.ApproximatePresenceCount.GetValueOrDefault();

    public string? Description => _jsonModel.Description;
}
