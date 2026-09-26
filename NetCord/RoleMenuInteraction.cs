using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class RoleMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntityMenuInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override RoleMenuInteractionData Data { get; } = new(jsonModel.Data!, jsonModel.GuildId, client);
}

public class RoleMenuInteractionData : EntityMenuInteractionData
{
    public unsafe RoleMenuInteractionData(JsonModels.JsonInteractionData jsonModel,
                                          ulong? guildId,
                                          RestClient client) : base(jsonModel,
                                                                    GetSelectedValues(jsonModel, guildId, client, &EntityMenuHelper.GetRoleValues, out var selectedValues, out var resolvedData),
                                                                    resolvedData)
    {
        SelectedValues = selectedValues;
    }

    public new IReadOnlyList<Role> SelectedValues { get; }
}
