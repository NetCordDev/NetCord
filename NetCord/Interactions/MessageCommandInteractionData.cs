using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessageCommandInteractionData : ApplicationCommandInteractionData
{
    public MessageCommandInteractionData(JsonInteractionData jsonModel, Snowflake? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        TargetMessage = new(jsonModel.ResolvedData!.Messages![jsonModel.TargetId.GetValueOrDefault()], client);
    }

    public RestMessage TargetMessage { get; }
}