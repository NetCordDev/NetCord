namespace NetCord;

public class MessageSticker : ClientEntity
{
    private readonly JsonModels.JsonMessageSticker _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public StickerFormat Format => _jsonEntity.Format;

    internal MessageSticker(JsonModels.JsonMessageSticker jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }
}