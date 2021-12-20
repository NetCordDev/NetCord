using System.Net.Mime;

namespace NetCord;

public class Attachment : Entity
{
    private protected readonly JsonModels.JsonAttachment _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;
    public string Filename => _jsonEntity.Filename;
    public string? Description => _jsonEntity.Description;
    public ContentType? ContentType => _jsonEntity.ContentType;
    public int Size => _jsonEntity.Size;
    public string Url => _jsonEntity.Url;
    public string ProxyUrl => _jsonEntity.ProxyUrl;
    public bool Ephemeral => _jsonEntity.Ephemeral;

    private protected Attachment(JsonModels.JsonAttachment jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    internal static Attachment CreateFromJson(JsonModels.JsonAttachment jsonEntity)
    {
        if (jsonEntity.Width.HasValue)
            return new ImageAttachment(jsonEntity);
        else
            return new Attachment(jsonEntity);
    }
}
