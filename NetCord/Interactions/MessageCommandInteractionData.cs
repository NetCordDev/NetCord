using NetCord.JsonModels;

namespace NetCord;

public class MessageCommandInteractionData : ApplicationCommandInteractionData
{
    internal MessageCommandInteractionData(JsonInteractionData jsonEntity, DiscordId? guildId, RestClient client) : base(jsonEntity, guildId, client)
    {
        TargetMessage = new(jsonEntity.ResolvedData!.Messages![jsonEntity.TargetId.GetValueOrDefault()], client);
    }

    public RestMessage TargetMessage { get; }
}