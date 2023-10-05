namespace NetCord.Rest;

public class Sku : Entity, IJsonModel<JsonModels.JsonSku>
{
    JsonModels.JsonSku IJsonModel<JsonModels.JsonSku>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonSku _jsonModel;

    public Sku(JsonModels.JsonSku jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public override ulong Id => _jsonModel.Id;

    public SkuType Type => _jsonModel.Type;

    public string Name => _jsonModel.Name;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public string Slug => _jsonModel.Slug;

    public SkuFlags Flags => _jsonModel.Flags;
}
