using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public abstract class EntityMenuInteraction : MessageComponentInteraction
{
    private protected EntityMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
    }

    public abstract override EntityMenuInteractionData Data { get; }
}

public abstract class EntityMenuInteractionData : MessageComponentInteractionData
{
    private protected EntityMenuInteractionData(JsonModels.JsonInteractionData jsonModel,
                                                IReadOnlyList<ulong> selectedValues,
                                                InteractionResolvedData? resolvedData) : base(jsonModel)
    {
        SelectedValues = selectedValues;

        ResolvedData = resolvedData;
    }

    private protected static unsafe IReadOnlyList<ulong> GetSelectedValues<T>(JsonModels.JsonInteractionData jsonModel,
                                                                              ulong? guildId,
                                                                              RestClient client,
                                                                              delegate*<string[], InteractionResolvedData, T[]> getSelectedValues,
                                                                              out T[] selectedValues,
                                                                              out InteractionResolvedData? resolvedData) where T : Entity
    {
        if (jsonModel.ResolvedData is { } jsonResolvedData)
            return new EntityArrayWrapper<T>(selectedValues = getSelectedValues(jsonModel.SelectedValues!, resolvedData = new(jsonResolvedData, guildId, client)));

        resolvedData = null;
        selectedValues = [];
        return [];
    }

    public IReadOnlyList<ulong> SelectedValues { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
