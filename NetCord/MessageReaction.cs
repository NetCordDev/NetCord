namespace NetCord;

public class MessageReaction(JsonModels.JsonMessageReaction jsonModel) : IJsonModel<JsonModels.JsonMessageReaction>
{
    JsonModels.JsonMessageReaction IJsonModel<JsonModels.JsonMessageReaction>.JsonModel => jsonModel;

    public int Count => jsonModel.Count;

    public MessageReactionCountDetails CountDetails { get; } = new(jsonModel.CountDetails);

    public bool Me => jsonModel.Me;

    public bool MeBurst => jsonModel.MeBurst;

    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);

    public IReadOnlyList<Color> BurstColors => jsonModel.BurstColors;
}
