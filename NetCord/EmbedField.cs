namespace NetCord;

/// <summary>
/// Contains information about an embed field, of which a maximum of 25 can be set per embed.
/// </summary>
public class EmbedField : IJsonModel<JsonModels.JsonEmbedField>
{
    JsonModels.JsonEmbedField IJsonModel<JsonModels.JsonEmbedField>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedField _jsonModel;
    public EmbedField(JsonModels.JsonEmbedField jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// Equivalent to <see cref="Embed.Title"/> but localised to a field, limited to 256 characters.
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// Equivalent to <see cref="Embed.Description"/> but localised to a field, limited to 1024 characters.
    /// </summary>
    
    public string Value => _jsonModel.Value;

    /// <summary>
    /// When set alongside another field with <see cref="Inline"/> set, displays the fields side by side.
    /// </summary>
    public bool Inline => _jsonModel.Inline;
}
