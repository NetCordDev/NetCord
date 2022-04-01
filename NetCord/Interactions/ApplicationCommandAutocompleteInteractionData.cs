namespace NetCord;

public class ApplicationCommandAutocompleteInteractionData : SlashCommandInteractionData
{
    internal ApplicationCommandAutocompleteInteractionData(JsonModels.JsonInteractionData jsonEntity, Snowflake? guildId, RestClient client) : base(jsonEntity, guildId, client)
    {
    }
}