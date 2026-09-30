using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public sealed class AutocompleteInteraction(JsonAutocompleteInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    public override AutocompleteInteractionData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

public sealed class AutocompleteInteractionData(JsonApplicationCommandInteractionData jsonModel, ulong? guildId, RestClient client) : SlashCommandInteractionData(jsonModel, guildId, client);
