using NetCord.Rest;

namespace NetCord;

public class MessageCommandInteractionData(JsonModels.JsonInteractionData jsonModel, RestClient client) : ApplicationCommandInteractionData(jsonModel)
{
    public RestMessage TargetMessage { get; } = new(jsonModel.ResolvedData!.Messages![jsonModel.TargetId.GetValueOrDefault()], client);
}
