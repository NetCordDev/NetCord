using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class SlashCommandInteractionData : ApplicationCommandInteractionData
{
    public SlashCommandInteractionData(JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToArray();
        if (jsonModel.ResolvedData != null)
            ResolvedData = new(jsonModel.ResolvedData, guildId, client);
    }

    public IReadOnlyList<ApplicationCommandInteractionDataOption> Options { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
