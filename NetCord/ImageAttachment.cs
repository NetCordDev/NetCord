namespace NetCord;

public class ImageAttachment : Attachment
{
    /// <summary>
    /// Height of file.
    /// </summary>
    public int Height => _jsonModel.Height.GetValueOrDefault();

    /// <summary>
    /// Width of file.
    /// </summary>
    public int Width => _jsonModel.Width.GetValueOrDefault();

    public ImageAttachment(JsonModels.JsonAttachment jsonModel) : base(jsonModel)
    {
    }
}
