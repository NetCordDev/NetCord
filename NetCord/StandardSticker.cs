namespace NetCord;

public class StandardSticker : Sticker
{
    public Snowflake PackId => _jsonModel.PackId.GetValueOrDefault();
    public int? SortValue => _jsonModel.SortValue;

    public StandardSticker(JsonModels.JsonSticker jsonModel) : base(jsonModel)
    {
    }
}
