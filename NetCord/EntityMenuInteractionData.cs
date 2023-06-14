using System.Globalization;

using NetCord.Rest;

namespace NetCord;

public class EntityMenuInteractionData : MessageComponentInteractionData
{
    public EntityMenuInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel)
    {
        var selectedValues = jsonModel.SelectedValues!;
        int length = selectedValues.Length;
        var result = new ulong[length];
        for (var i = 0; i < length; i++)
            result[i] = ulong.Parse(selectedValues[i], NumberStyles.None, CultureInfo.InvariantCulture);
        SelectedValues = result;

        var resolvedData = jsonModel.ResolvedData;
        if (resolvedData is not null)
            ResolvedData = new(resolvedData, guildId, client);
    }

    public IReadOnlyList<ulong> SelectedValues { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
