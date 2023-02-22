namespace NetCord.Gateway;

public class ApplicationCommandInteractionData : InteractionData, IEntity
{
    public ulong Id => _jsonModel.Id.GetValueOrDefault();

    public string Name => _jsonModel.Name!;

    public ApplicationCommandType Type => _jsonModel.Type.GetValueOrDefault();

    public ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }
}
