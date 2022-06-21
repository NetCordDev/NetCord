namespace NetCord;

public class ImageAttachment : Attachment
{
    public int Height => _jsonModel.Height.GetValueOrDefault();
    public int Width => _jsonModel.Width.GetValueOrDefault();

    public ImageAttachment(JsonModels.JsonAttachment jsonModel) : base(jsonModel)
    {
    }
}