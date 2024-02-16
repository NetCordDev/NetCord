namespace NetCord.Rest;

public class GuildWidgetChannel(JsonModels.JsonGuildWidgetChannel jsonModel) : Entity, IJsonModel<JsonModels.JsonGuildWidgetChannel>
{
    JsonModels.JsonGuildWidgetChannel IJsonModel<JsonModels.JsonGuildWidgetChannel>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    public string Name => jsonModel.Name;

    public int Position => jsonModel.Position;
}
