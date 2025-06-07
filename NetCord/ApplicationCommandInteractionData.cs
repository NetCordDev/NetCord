namespace NetCord;

/// <summary>
/// Contains data for an invoked <see cref="Rest.ApplicationCommand"/>.
/// </summary>
public class ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonModel) : InteractionData(jsonModel)
{
    /// <summary>
    /// The invoked <see cref="Rest.ApplicationCommand"/>'s ID.
    /// </summary>
    public ulong Id => _jsonModel.Id.GetValueOrDefault();

    /// <summary>
    /// The invoked <see cref="Rest.ApplicationCommand"/>'s name.
    /// </summary>
    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The invoked <see cref="Rest.ApplicationCommand"/>'s type.
    /// </summary>
    public ApplicationCommandType Type => _jsonModel.Type.GetValueOrDefault();

    /// <summary>
    /// ID of the guild the command is registered to.
    /// </summary>
    public ulong? GuildId => _jsonModel.GuildId;
}
