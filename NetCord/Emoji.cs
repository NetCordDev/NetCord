namespace NetCord;

public class Emoji
{
    private protected readonly JsonModels.JsonEmoji _jsonEntity;

    public string Name => _jsonEntity.Name!;

    public bool Animated => _jsonEntity.Animated;

    internal Emoji(JsonModels.JsonEmoji jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public override string ToString() => Name;

    internal static Emoji CreateFromJson(JsonModels.JsonEmoji jsonEntity, Snowflake guildId, RestClient client)
    {
        if (jsonEntity.Id.HasValue)
            return new GuildEmoji(jsonEntity, guildId, client);
        else
            return new Emoji(jsonEntity);
    }
}