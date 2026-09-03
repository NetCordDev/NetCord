namespace NetCord;

/// <summary>
/// Represents count information for a <see cref="MessageReaction"/> object.
/// </summary>
public class MessageReactionCountDetails(JsonModels.JsonMessageReactionCountDetails jsonModel) : IJsonModel<JsonModels.JsonMessageReactionCountDetails>
{
    JsonModels.JsonMessageReactionCountDetails IJsonModel<JsonModels.JsonMessageReactionCountDetails>.JsonModel => jsonModel;

    /// <summary>
    /// The number of applied ssuper reactions.
    /// </summary>
    public int Burst => jsonModel.Burst;

    /// <summary>
    /// The number of applied normal reactions.
    /// </summary>
    public int Normal => jsonModel.Normal;
}
