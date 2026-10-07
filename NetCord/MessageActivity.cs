using NetCord.JsonModels;

namespace NetCord;

public class MessageActivity(JsonMessageActivity jsonModel)
{
    public MessageActivityType Type { get; } = jsonModel.Type;

    public string? PartyId { get; } = jsonModel.PartyId;
}
