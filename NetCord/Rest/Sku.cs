namespace NetCord.Rest;

public class Sku(JsonModels.JsonSku jsonModel) : Entity, IJsonModel<JsonModels.JsonSku>
{
    JsonModels.JsonSku IJsonModel<JsonModels.JsonSku>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Type of the SKU.
    /// </summary>
    public SkuType Type => jsonModel.Type;

    /// <summary>
    /// ID of the parent application.
    /// </summary>
    public ulong ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// Customer-facing name of your premium offering.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// System-generated URL slug based on the SKU's name.
    /// </summary>
    public string Slug => jsonModel.Slug;

    /// <summary>
    /// Flags of the SKU.
    /// </summary>
    public SkuFlags Flags => jsonModel.Flags;
}
