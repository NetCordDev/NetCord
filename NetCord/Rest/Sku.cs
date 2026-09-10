namespace NetCord.Rest;

/// <summary>
/// Represents a premium offering that can be made to application users/guilds.
/// </summary>
public partial class Sku(JsonModels.JsonSku jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonSku>
{
    JsonModels.JsonSku IJsonModel<JsonModels.JsonSku>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The SKU's purchase type.
    /// </summary>
    public SkuType Type => jsonModel.Type;

    /// <summary>
    /// The ID corresponding to the SKU's parent application.
    /// </summary>
    public ulong ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// The SKU's customer-facing name.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The system-generated URL slug, based on the SKU's name.
    /// </summary>
    public string Slug => jsonModel.Slug;

    /// <summary>
    /// The SKU's content flags.
    /// </summary>
    public SkuFlags Flags => jsonModel.Flags;
}
