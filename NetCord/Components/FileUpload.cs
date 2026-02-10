using NetCord.JsonModels;

namespace NetCord;

public class FileUpload : IInteractiveComponent, ILabelComponent, IJsonModel<JsonFileUploadComponent>
{
    JsonFileUploadComponent IJsonModel<JsonFileUploadComponent>.JsonModel => _jsonModel;
    private readonly JsonFileUploadComponent _jsonModel;

    public FileUpload(JsonFileUploadComponent jsonModel, InteractionResolvedData? resolvedData)
    {
        _jsonModel = jsonModel;

        Attachments = resolvedData is { Attachments: var attachments }
            ? jsonModel.Values.Select(id => attachments![id]).ToArray()
            : [];
    }

    public int Id => _jsonModel.Id;
    public string CustomId => _jsonModel.CustomId!;
    public IReadOnlyList<Attachment> Attachments { get; }
}
