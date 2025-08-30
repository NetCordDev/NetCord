using NetCord.Rest;

namespace NetCord;

public class ModalInteractionData : ComponentInteractionData
{
    public ModalInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel)
    {
        Components = jsonModel.Components!.Select(IModalComponent.CreateFromJson).ToArray();

        var resolvedData = jsonModel.ResolvedData;
        if (resolvedData is not null)
            ResolvedData = new(resolvedData, guildId, client);
    }

    public IReadOnlyList<IModalComponent> Components { get; }

    public InteractionResolvedData? ResolvedData { get; }
}
