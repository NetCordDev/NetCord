namespace NetCord;

public class Party
{
    private readonly JsonModels.JsonParty _jsonEntity;

    public string? Id => _jsonEntity.Id;

    public int? CurrentSize { get; }

    public int? MaxSize { get; }

    internal Party(JsonModels.JsonParty jsonEntity)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Size != null)
        {
            CurrentSize = jsonEntity.Size[0];
            MaxSize = jsonEntity.Size[1];
        }
    }
}