using NetCord.JsonModels;

namespace NetCord;

public abstract class Menu(JsonComponent jsonModel) : IMessageComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private protected readonly JsonComponent _jsonModel = jsonModel;

    public string CustomId => _jsonModel.CustomId!;
    public string? Placeholder => _jsonModel.Placeholder;
    public int? MinValues => _jsonModel.MinValues;
    public int? MaxValues => _jsonModel.MaxValues;
    public bool Disabled => _jsonModel.Disabled.GetValueOrDefault();

    public static Menu CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.StringMenu => new StringMenu(jsonModel),
            ComponentType.UserMenu => new UserMenu(jsonModel),
            ComponentType.ChannelMenu => new ChannelMenu(jsonModel),
            ComponentType.RoleMenu => new RoleMenu(jsonModel),
            ComponentType.MentionableMenu => new MentionableMenu(jsonModel),
            _ => throw new NotImplementedException(),
        };
    }
}
