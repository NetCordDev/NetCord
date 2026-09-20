using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a file upload interaction component.
/// </summary>
public class FileUpload(JsonFileUploadComponent jsonModel, InteractionResolvedData? resolvedData) 
    : IInteractiveComponent, ILabelComponent, IJsonModel<JsonFileUploadComponent>
{
    JsonFileUploadComponent IJsonModel<JsonFileUploadComponent>.JsonModel => jsonModel;

    /// <summary>
    /// The unique integer ID of the component.
    /// </summary>
    public int Id => jsonModel.Id;

    /// <summary>
    /// The developer-defined identifier for the file upload component.
    /// </summary>
    public string CustomId => jsonModel.CustomId;

    /// <summary>
    /// The collection of uploaded attachments resolved from the interaction context.
    /// </summary>
    public IReadOnlyList<Attachment> Attachments { get; } = resolvedData is { Attachments: { } attachments }
        ? jsonModel.Values.Select(id => attachments[id]).ToArray()
        : [];
}
