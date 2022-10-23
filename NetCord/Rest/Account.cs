namespace NetCord.Rest;

public class Account : Entity, IJsonModel<JsonModels.JsonAccount>
{
    JsonModels.JsonAccount IJsonModel<JsonModels.JsonAccount>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonAccount _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public Account(JsonModels.JsonAccount jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
