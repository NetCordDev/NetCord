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

        var size = jsonModel.Size;
        if (size is not null)
        {
            CurrentSize = size[0];
            MaxSize = size[1];
        }
    }
}
