namespace NetCord.Rest;

/// <summary>
/// Represents a premium offering that can be made to application users/guilds.
/// </summary>
public partial class Sku(JsonModels.JsonSku jsonModel, RestClient client) : ClientEntity(client)
{
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// The SKU's purchase type.
    /// </summary>
    public SkuType Type { get; } = jsonModel.Type;

    /// <summary>
    /// The ID corresponding to the SKU's parent application.
    /// </summary>
    public ulong ApplicationId { get; } = jsonModel.ApplicationId;

    /// <summary>
    /// The SKU's customer-facing name.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// The system-generated URL slug, based on the SKU's name.
    /// </summary>
    public string Slug { get; } = jsonModel.Slug;

    /// <summary>
    /// The SKU's content flags.
    /// </summary>
    public SkuFlags Flags { get; } = jsonModel.Flags;
}
