namespace NetCord;

public abstract class MessageEmbedPartBase
{
    private readonly JsonModels.JsonEmbedPartBase _jsonEntity;

    public string? Url => _jsonEntity.Url;
    public string? ProxyUrl => _jsonEntity.ProxyUrl;
    public int? Height => _jsonEntity.Height;
    public int? Width => _jsonEntity.Width;

    internal MessageEmbedPartBase(JsonModels.JsonEmbedPartBase jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
