namespace NetCord;

public class Role : ClientEntity
{
    private readonly JsonModels.JsonRole _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public Color Color => _jsonEntity.Color;

    public bool Hoist => _jsonEntity.Hoist;

    public int Position => _jsonEntity.Position;

    public Permission Permissions { get; }

    public bool Managed => _jsonEntity.Managed;

    public bool Mentionable => _jsonEntity.Mentionable;

    public Tags? Tags { get; }

    internal Role(JsonModels.JsonRole jsonEntity, BotClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Tags != null)
            Tags = new(jsonEntity.Tags);
        Permissions = (Permission)ulong.Parse(jsonEntity.Permissions);
    }

    public override string ToString() => $"<@&{Id}>";
}

public class Tags
{
    private readonly JsonModels.JsonTags _jsonEntity;

    public DiscordId? BotId => _jsonEntity.BotId;

    public DiscordId? IntegrationId => _jsonEntity.IntegrationId;

    public bool IsPremiumSubscriber => _jsonEntity.IsPremiumSubscriber;

    internal Tags(JsonModels.JsonTags jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}