using NetCord.JsonModels;

namespace NetCord;

public interface IComponentSectionAccessory
{
    public static IComponentSectionAccessory CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.Button => IButton.CreateFromJson(jsonModel),
            ComponentType.Thumbnail => new ComponentSectionThumbnail(jsonModel),
            _ => new UnknownComponent(jsonModel),
        };
    }
}
