using System.Collections.ObjectModel;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class SlashCommandInteractionData : ApplicationCommandInteractionData
{
    public SlashCommandInteractionData(JsonInteractionData jsonModel, Snowflake? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        Options = new(jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToList());
        if (jsonModel.ResolvedData != null)
            ResolvedData = new(jsonModel.ResolvedData, guildId, client);
    }

    public ReadOnlyCollection<ApplicationCommandInteractionDataOption> Options { get; }

    public SlashCommandInteractionResolvedData? ResolvedData { get; }
}