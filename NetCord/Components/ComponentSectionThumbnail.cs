using NetCord.JsonModels;

namespace NetCord;

public class ComponentSectionThumbnail(JsonComponent jsonModel) : IComponentSectionAccessoryComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public ComponentMedia Media { get; } = new(jsonModel.Media!);
    public string? Description => jsonModel.Description;
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
}
