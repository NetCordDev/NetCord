using NetCord.Rest;

namespace NetCord.Gateway;

public class ApplicationCommandInteractionData : InteractionData, IEntity
{
    public Snowflake Id => _jsonModel.Id.GetValueOrDefault();

    public string Name => _jsonModel.Name!;

    public ApplicationCommandType Type => _jsonModel.Type.GetValueOrDefault();

    public ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonModel, Snowflake? guildId, RestClient client) : base(jsonModel)
    {
    }
}
