namespace NetCord.Gateway;

public class Party : IJsonModel<JsonModels.JsonParty>
{
    JsonModels.JsonParty IJsonModel<JsonModels.JsonParty>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonParty _jsonModel;

    public string? Id => _jsonModel.Id;

    public int? CurrentSize { get; }

    public int? MaxSize { get; }

    public Party(JsonModels.JsonParty jsonModel)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Size != null)
        {
            CurrentSize = jsonModel.Size[0];
            MaxSize = jsonModel.Size[1];
        }
    }
}
