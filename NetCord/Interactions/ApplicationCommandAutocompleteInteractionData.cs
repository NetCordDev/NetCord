namespace NetCord;

public class ApplicationCommandAutocompleteInteractionData : SlashCommandInteractionData
{
    internal ApplicationCommandAutocompleteInteractionData(JsonModels.JsonInteractionData jsonEntity, DiscordId? guildId, RestClient client) : base(jsonEntity, guildId, client)
    {
    }
}