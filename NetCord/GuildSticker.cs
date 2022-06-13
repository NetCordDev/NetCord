namespace NetCord;

public class GuildSticker : Sticker
{
    private readonly RestClient _client;

    public bool? Available => _jsonModel.Available;

    public Snowflake GuildId => _jsonModel.GuildId;

    public User? Creator { get; }

    public GuildSticker(JsonModels.JsonSticker jsonModel, RestClient client) : base(jsonModel)
    {
        _client = client;
        if (jsonModel.Creator != null)
            Creator = new(jsonModel.Creator, client);
    }
}