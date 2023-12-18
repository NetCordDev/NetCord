namespace NetCord;

/// <summary>
/// Contains information used to display the provider tag at the top of an embed.
/// </summary>
public class EmbedProvider : IJsonModel<JsonModels.JsonEmbedProvider>
{
    JsonModels.JsonEmbedProvider IJsonModel<JsonModels.JsonEmbedProvider>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedProvider _jsonModel;
    public EmbedProvider(JsonModels.JsonEmbedProvider jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// The name of the provider, displayed at the top of the embed.
    /// </summary>
    public string? Name => _jsonModel.Name;

    /// <summary>
    /// When set, turns the <see cref="Name"/> into a clickable link, pointing to the base of the specified URL.
    /// </summary>
    public string? Url => _jsonModel.Url;
}
