namespace NetCord;

public class ApplicationCommandInteractionData : InteractionData, IEntity
{
    public DiscordId Id => _jsonEntity.Id.GetValueOrDefault();

    public string Name => _jsonEntity.Name!;

    public ApplicationCommandType Type => _jsonEntity.Type.GetValueOrDefault();

    internal ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonEntity, DiscordId? guildId, RestClient client) : base(jsonEntity)
    {
    }
}