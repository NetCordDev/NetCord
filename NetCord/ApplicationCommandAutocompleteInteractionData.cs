namespace NetCord;

public class ApplicationCommandAutocompleteInteractionData : ApplicationCommandInteractionData
{
    internal ApplicationCommandAutocompleteInteractionData(JsonModels.JsonInteractionData jsonEntity, DiscordId? guildId, RestClient client) : base(jsonEntity, guildId, client)
    {
    }
}