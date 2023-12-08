using NetCord.Rest;

namespace NetCord;

public class AutocompleteInteractionData : SlashCommandInteractionData
{
    public AutocompleteInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
    }
}
