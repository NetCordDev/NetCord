using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class SlashCommandInteractionData : ApplicationCommandInteractionData
{
    public SlashCommandInteractionData(JsonInteractionData jsonModel, Snowflake? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToArray();
        if (jsonModel.ResolvedData != null)
            ResolvedData = new(jsonModel.ResolvedData, guildId, client);
    }

    public IReadOnlyList<ApplicationCommandInteractionDataOption> Options { get; }

    public SlashCommandInteractionResolvedData? ResolvedData { get; }
}
