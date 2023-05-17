using NetCord.Rest;

namespace NetCord.Gateway;

public class SlashCommandInteractionData : ApplicationCommandInteractionData
{
    public SlashCommandInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel)
    {
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToArray();
        if (jsonModel.ResolvedData != null)
            ResolvedData = new(jsonModel.ResolvedData, guildId, client);
    }

    public IReadOnlyList<ApplicationCommandInteractionDataOption> Options { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
