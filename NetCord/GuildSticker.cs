namespace NetCord;

public class GuildSticker : Sticker
{
    private readonly RestClient _client;

    public bool? Available => _jsonEntity.Available;

    public DiscordId GuildId => _jsonEntity.GuildId;

    public User Creator { get; }

    internal GuildSticker(JsonModels.JsonSticker jsonEntity, RestClient client) : base(jsonEntity)
    {
        _client = client;
        Creator = new(jsonEntity.Creator!, client);
    }
}