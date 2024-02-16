using NetCord.Rest;

namespace NetCord;

public class AutocompleteInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : SlashCommandInteractionData(jsonModel, guildId, client)
{
}
