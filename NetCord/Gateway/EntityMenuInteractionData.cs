using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class EntityMenuInteractionData : ButtonInteractionData
{
    public EntityMenuInteractionData(JsonInteractionData jsonModel, Snowflake? guildId, RestClient client) : base(jsonModel)
    {
        int length = jsonModel.SelectedValues!.Length;
        var selectedValues = new Snowflake[length];
        for (int i = 0; i < length; i++)
            selectedValues[i] = new(jsonModel.SelectedValues[i]);
        SelectedValues = selectedValues;
        if (jsonModel.ResolvedData != null)
            ResolvedData = new(jsonModel.ResolvedData, guildId, client);
    }

    public IReadOnlyList<Snowflake> SelectedValues { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
