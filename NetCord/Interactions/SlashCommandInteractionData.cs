using System.Collections.ObjectModel;

using NetCord.JsonModels;

namespace NetCord;

public class SlashCommandInteractionData : ApplicationCommandInteractionData
{
    internal SlashCommandInteractionData(JsonInteractionData jsonEntity, Snowflake? guildId, RestClient client) : base(jsonEntity, guildId, client)
    {
        Options = new(jsonEntity.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToList());
        if (jsonEntity.ResolvedData != null)
            ResolvedData = new(jsonEntity.ResolvedData, guildId, client);
    }

    public ReadOnlyCollection<ApplicationCommandInteractionDataOption> Options { get; }

    public SlashCommandInteractionResolvedData? ResolvedData { get; }
}