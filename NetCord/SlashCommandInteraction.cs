using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class SlashCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override SlashCommandInteractionData Data { get; } = new(jsonModel.Data!, jsonModel.GuildId, client);
}

public class SlashCommandInteractionData : ApplicationCommandInteractionData
{
    public SlashCommandInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel)
    {
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToArray();

        var resolvedData = jsonModel.ResolvedData;
        if (resolvedData is not null)
            ResolvedData = new(resolvedData, guildId, client);
    }

    public IReadOnlyList<ApplicationCommandInteractionDataOption> Options { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
