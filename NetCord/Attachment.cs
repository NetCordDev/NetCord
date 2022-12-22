using System.Net.Mime;

namespace NetCord;

public class Attachment : Entity, IJsonModel<JsonModels.JsonAttachment>
{
    JsonModels.JsonAttachment IJsonModel<JsonModels.JsonAttachment>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonAttachment _jsonModel;

    public override ulong Id => _jsonModel.Id;
    public string FileName => _jsonModel.FileName;
    public string? Description => _jsonModel.Description;
    public ContentType? ContentType => _jsonModel.ContentType;
    public int Size => _jsonModel.Size;
    public string Url => _jsonModel.Url;
    public string ProxyUrl => _jsonModel.ProxyUrl;
    public bool Ephemeral => _jsonModel.Ephemeral;

    private protected Attachment(JsonModels.JsonAttachment jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public static Attachment CreateFromJson(JsonModels.JsonAttachment jsonModel)
    {
        if (jsonModel.Width.HasValue)
            return new ImageAttachment(jsonModel);
        else
            return new Attachment(jsonModel);
    }
}
