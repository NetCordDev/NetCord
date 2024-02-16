namespace NetCord;

public class ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonModel) : InteractionData(jsonModel)
{
    public ulong Id => _jsonModel.Id.GetValueOrDefault();

    public string Name => _jsonModel.Name!;

    public ApplicationCommandType Type => _jsonModel.Type.GetValueOrDefault();
}
