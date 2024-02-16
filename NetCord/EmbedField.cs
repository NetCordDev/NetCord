namespace NetCord;

/// <summary>
/// Contains information about an embed field, of which a maximum of 25 can be set per embed.
/// </summary>
public class EmbedField(JsonModels.JsonEmbedField jsonModel) : IJsonModel<JsonModels.JsonEmbedField>
{
    JsonModels.JsonEmbedField IJsonModel<JsonModels.JsonEmbedField>.JsonModel => jsonModel;

    /// <summary>
    /// Equivalent to <see cref="Embed.Title"/> but localised to a field, limited to 256 characters.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// Equivalent to <see cref="Embed.Description"/> but localised to a field, limited to 1024 characters.
    /// </summary>
    public string Value => jsonModel.Value;

    /// <summary>
    /// When set alongside another field with <see cref="Inline"/> set, displays the fields side by side.
    /// </summary>
    public bool Inline => jsonModel.Inline;
}
