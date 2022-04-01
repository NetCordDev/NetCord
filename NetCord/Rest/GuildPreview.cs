using System.Collections.Immutable;

namespace NetCord;

public class GuildPreview : ClientEntity
{
    private readonly JsonModels.JsonGuild _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public string? IconHash => _jsonEntity.IconHash;

    public string? SplashHash => _jsonEntity.SplashHash;

    public string? DiscoverySplashHash => _jsonEntity.DiscoverySplashHash;

    public ImmutableDictionary<Snowflake, GuildEmoji> Emojis { get; }

    public GuildFeatures Features { get; }

    public int ApproximateMemberCount => _jsonEntity.ApproximateMemberCount.GetValueOrDefault();

    public int ApproximatePresenceCount => _jsonEntity.ApproximatePresenceCount.GetValueOrDefault();

    public string? Description => _jsonEntity.Description;

    internal GuildPreview(JsonModels.JsonGuild jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
        Emojis = _jsonEntity.Emojis.ToImmutableDictionaryOrEmpty(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, Id, client));
        Features = new(jsonEntity.Features);
    }
}