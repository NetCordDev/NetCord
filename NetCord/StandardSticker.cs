namespace NetCord;

public class StandardSticker(JsonModels.JsonSticker jsonModel) : Sticker(jsonModel)
{
    public ulong PackId => _jsonModel.PackId.GetValueOrDefault();
    public int? SortValue => _jsonModel.SortValue;
}
