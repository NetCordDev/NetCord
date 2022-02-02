namespace NetCord;

public class EmbedFooter
{
    private readonly JsonModels.JsonEmbedFooter _jsonEntity;

    public string Text => _jsonEntity.Text;
    public string? IconUrl => _jsonEntity.IconUrl;
    public string? ProxyIconUrl => _jsonEntity.ProxyIconUrl;

    internal EmbedFooter(JsonModels.JsonEmbedFooter jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
