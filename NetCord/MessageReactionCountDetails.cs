namespace NetCord;

public class MessageReactionCountDetails : IJsonModel<JsonModels.JsonMessageReactionCountDetails>
{
    JsonModels.JsonMessageReactionCountDetails IJsonModel<JsonModels.JsonMessageReactionCountDetails>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMessageReactionCountDetails _jsonModel;

    public MessageReactionCountDetails(JsonModels.JsonMessageReactionCountDetails jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// Count of super reactions.
    /// </summary>
    public int Burst => _jsonModel.Burst;

    /// <summary>
    /// Count of normal reactions.
    /// </summary>
    public int Normal => _jsonModel.Normal;
}
