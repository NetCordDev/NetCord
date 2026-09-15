namespace NetCord.Gateway;

/// <summary>
/// Represents information about a user's presence party.
/// </summary>
public class Party(JsonModels.JsonParty jsonModel) : IJsonModel<JsonModels.JsonParty>
{
    JsonModels.JsonParty IJsonModel<JsonModels.JsonParty>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the party.
    /// </summary>
    public string? Id => jsonModel.Id;

    /// <summary>
    /// The size of the party, containing current and max party size.
    /// </summary>
    public PartySize? Size { get; } = jsonModel.Size is { } size ? new(size) : null;
}
