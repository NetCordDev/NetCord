namespace NetCord;

public class MessageReactionCountDetails(JsonModels.JsonMessageReactionCountDetails jsonModel) : IJsonModel<JsonModels.JsonMessageReactionCountDetails>
{
    JsonModels.JsonMessageReactionCountDetails IJsonModel<JsonModels.JsonMessageReactionCountDetails>.JsonModel => jsonModel;

    /// <summary>
    /// Count of super reactions.
    /// </summary>
    public int Burst => jsonModel.Burst;

    /// <summary>
    /// Count of normal reactions.
    /// </summary>
    public int Normal => jsonModel.Normal;
}
