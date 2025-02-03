namespace NetCord;

/// <summary>
/// Contains information on an <see cref="ApplicationCommandInteraction"/>.
/// </summary>
public class ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonModel) : InteractionData(jsonModel)
{
    /// <summary>
    /// The parent <see cref="ApplicationCommandInteraction"/>'s ID.
    /// </summary>
    public ulong Id => _jsonModel.Id.GetValueOrDefault();

    /// <summary>
    /// The parent <see cref="ApplicationCommandInteraction"/>'s name.
    /// </summary>
    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The parent <see cref="ApplicationCommandInteraction"/>'s type.
    /// </summary>
    public ApplicationCommandType Type => _jsonModel.Type.GetValueOrDefault();
}
