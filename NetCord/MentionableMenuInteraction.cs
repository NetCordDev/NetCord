using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class MentionableMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntityMenuInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override MentionableMenuInteractionData Data { get; } = new(jsonModel.Data!, jsonModel.GuildId, client);
}

public class MentionableMenuInteractionData : EntityMenuInteractionData
{
    public unsafe MentionableMenuInteractionData(JsonModels.JsonInteractionData jsonModel,
                                                 ulong? guildId,
                                                 RestClient client) : base(jsonModel,
                                                                           GetSelectedValues(jsonModel, guildId, client, &EntityMenuHelper.GetMentionableValues, out var selectedValues, out var resolvedData),
                                                                           resolvedData)
    {
        SelectedValues = selectedValues;
    }

    public new IReadOnlyList<Mentionable> SelectedValues { get; }
}
