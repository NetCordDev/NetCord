namespace NetCord;

/// <summary>
/// Contains information used to display the provider tag at the top of an embed.
/// </summary>
public class EmbedProvider(JsonModels.JsonEmbedProvider jsonModel) : IJsonModel<JsonModels.JsonEmbedProvider>
{
    JsonModels.JsonEmbedProvider IJsonModel<JsonModels.JsonEmbedProvider>.JsonModel => jsonModel;

    /// <summary>
    /// The name of the provider, displayed at the top of the embed.
    /// </summary>
    public string? Name => jsonModel.Name;

    /// <summary>
    /// When set, turns the <see cref="Name"/> into a clickable link, pointing to the base of the specified URL.
    /// </summary>
    public string? Url => jsonModel.Url;
}
