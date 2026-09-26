namespace NetCord.Gateway;

public class Party : IJsonModel<JsonModels.JsonParty>
{
    JsonModels.JsonParty IJsonModel<JsonModels.JsonParty>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonParty _jsonModel;

    public string? Id => _jsonModel.Id;

    public PartySize? Size { get; }

    public Party(JsonModels.JsonParty jsonModel)
    {
        _jsonModel = jsonModel;

        if (jsonModel.Size is { } size)
            Size = new(size);
    }
}
