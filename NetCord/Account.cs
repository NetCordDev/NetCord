namespace NetCord;

/// <summary>
/// Represents an account as part of an <see cref="Integration"/> object.
/// </summary>
/// <param name="jsonModel">The JSON model to create an <see cref="Integration"/> object from.</param>
public class Account(JsonModels.JsonAccount jsonModel) : Entity, IJsonModel<JsonModels.JsonAccount>
{
    JsonModels.JsonAccount IJsonModel<JsonModels.JsonAccount>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the account.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The name of the account.
    /// </summary>
    public string Name => jsonModel.Name;
}
