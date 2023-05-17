using NetCord.Rest;

namespace NetCord.Gateway;

public class MessageCommandInteractionData : ApplicationCommandInteractionData
{
    public MessageCommandInteractionData(JsonModels.JsonInteractionData jsonModel, RestClient client) : base(jsonModel)
    {
        TargetMessage = new(jsonModel.ResolvedData!.Messages![jsonModel.TargetId.GetValueOrDefault()], client);
    }

    public RestMessage TargetMessage { get; }
}
