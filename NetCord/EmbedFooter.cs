namespace NetCord;

/// <summary>
/// Contains information used to render the footer block of an embed.
/// </summary>
public class EmbedFooter : IJsonModel<JsonModels.JsonEmbedFooter>
{
    JsonModels.JsonEmbedFooter IJsonModel<JsonModels.JsonEmbedFooter>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedFooter _jsonModel;
    public EmbedFooter(JsonModels.JsonEmbedFooter jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// The text displayed in the footer, to the right of the icon if one is set, limited to 2048 characters.
    /// </summary>
    public string Text => _jsonModel.Text;
    
    /// <summary>
    /// Points to an image, which is displayed in a small circular format to the left of the <see cref="Text"/>.
    /// </summary>
    public string? IconUrl => _jsonModel.IconUrl;
    
    /// <summary>
    /// The URL of the icon image, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyIconUrl => _jsonModel.ProxyIconUrl;
}
