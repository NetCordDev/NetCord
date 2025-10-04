using NetCord.JsonModels;

namespace NetCord;

public class FileUpload : IInteractiveComponent, ILabelComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel;

    public FileUpload(JsonComponent jsonModel, InteractionResolvedData? resolvedData)
    {
        _jsonModel = jsonModel;

        Attachments = resolvedData is { Attachments: var attachments }
            ? jsonModel.SelectedValues!.Select(id => attachments![Snowflake.Parse(id)]).ToArray()
            : [];
    }

    public int Id => _jsonModel.Id;
    public string CustomId => _jsonModel.CustomId!;
    public IReadOnlyList<Attachment> Attachments { get; }
}
