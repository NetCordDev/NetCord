using NetCord.Rest;

namespace NetCord.Gateway;

public class ApplicationCommandAutocompleteInteractionData : SlashCommandInteractionData
{
    public ApplicationCommandAutocompleteInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
    }
}
