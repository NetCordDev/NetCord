namespace NetCord;

public class GuildWidgetChannel : Entity, IJsonModel<JsonModels.JsonGuildWidgetChannel>
{
    JsonModels.JsonGuildWidgetChannel IJsonModel<JsonModels.JsonGuildWidgetChannel>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildWidgetChannel _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public int Position => _jsonModel.Position;

    public GuildWidgetChannel(JsonModels.JsonGuildWidgetChannel jsonModel)
    {
        _jsonModel = jsonModel;
    }
}