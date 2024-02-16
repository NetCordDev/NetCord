namespace NetCord;

public class Account(JsonModels.JsonAccount jsonModel) : Entity, IJsonModel<JsonModels.JsonAccount>
{
    JsonModels.JsonAccount IJsonModel<JsonModels.JsonAccount>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Name of the account.
    /// </summary>
    public string Name => jsonModel.Name;
}
