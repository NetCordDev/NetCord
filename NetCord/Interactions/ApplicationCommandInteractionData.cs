namespace NetCord;

public class ApplicationCommandInteractionData : InteractionData, IEntity
{
    public Snowflake Id => _jsonEntity.Id.GetValueOrDefault();

    public string Name => _jsonEntity.Name!;

    public ApplicationCommandType Type => _jsonEntity.Type.GetValueOrDefault();

    internal ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonEntity, Snowflake? guildId, RestClient client) : base(jsonEntity)
    {
    }
}