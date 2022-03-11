namespace NetCord;

public class GuildRole : ClientEntity
{
    private readonly JsonModels.JsonGuildRole _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public Color Color => _jsonEntity.Color;

    public bool Hoist => _jsonEntity.Hoist;

    public int Position => _jsonEntity.Position;

    public Permission Permissions { get; }

    public bool Managed => _jsonEntity.Managed;

    public bool Mentionable => _jsonEntity.Mentionable;

    public GuildRoleTags? Tags { get; }

    internal GuildRole(JsonModels.JsonGuildRole jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Tags != null)
            Tags = new(jsonEntity.Tags);
        Permissions = (Permission)ulong.Parse(jsonEntity.Permissions);
    }

    public override string ToString() => $"<@&{Id}>";

    public static bool operator >(GuildRole left, GuildRole right) => left.Position > right.Position;

    public static bool operator <(GuildRole left, GuildRole right) => left.Position < right.Position;

    public static bool operator >=(GuildRole left, GuildRole right) => left.Position >= right.Position;

    public static bool operator <=(GuildRole left, GuildRole right) => left.Position <= right.Position;
}

public class GuildRoleTags
{
    private readonly JsonModels.JsonTags _jsonEntity;

    public DiscordId? BotId => _jsonEntity.BotId;

    public DiscordId? IntegrationId => _jsonEntity.IntegrationId;

    public bool IsPremiumSubscriber => _jsonEntity.IsPremiumSubscriber;

    internal GuildRoleTags(JsonModels.JsonTags jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}