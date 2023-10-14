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

    /// <summary>
    /// Type of the SKU.
    /// </summary>
    public SkuType Type => _jsonModel.Type;

    /// <summary>
    /// Id of the parent application.
    /// </summary>
    public ulong ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Customer-facing name of your premium offering.
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// System-generated URL slug based on the SKU's name.
    /// </summary>
    public string Slug => _jsonModel.Slug;

    /// <summary>
    /// Flags of the SKU.
    /// </summary>
    public SkuFlags Flags => _jsonModel.Flags;
}
