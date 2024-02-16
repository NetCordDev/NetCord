namespace NetCord;

/// <summary>
/// Contains information about the author of an embed, used to render the author block.
/// </summary>
public class EmbedAuthor(JsonModels.JsonEmbedAuthor jsonModel) : IJsonModel<JsonModels.JsonEmbedAuthor>
{
    JsonModels.JsonEmbedAuthor IJsonModel<JsonModels.JsonEmbedAuthor>.JsonModel => jsonModel;

    /// <summary>
    /// The name of the author, displayed next to the icon if one is specified in <see cref="IconUrl"/>.
    /// </summary>
    public string? Name => jsonModel.Name;

    /// <summary>
    /// When set, turns the <see cref="Name"/> into a clickable link, pointing to the specified URL.
    /// </summary>
    public string? Url => jsonModel.Url;

    /// <summary>
    /// Points to an image, which is displayed in a small circular format to the left of the <see cref="Name"/>.
    /// </summary>
    public string? IconUrl => jsonModel.IconUrl;

    /// <summary>
    /// The URL of the icon image, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyIconUrl => jsonModel.ProxyIconUrl;
}
