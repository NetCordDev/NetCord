namespace NetCord;

public class ImageAttachment : Attachment
{
    public ImageAttachment(JsonModels.JsonAttachment jsonModel) : base(jsonModel)
    {
    }

    /// <summary>
    /// Height of file.
    /// </summary>
    public int Height => _jsonModel.Height.GetValueOrDefault();

    /// <summary>
    /// Width of file.
    /// </summary>
    public int Width => _jsonModel.Width.GetValueOrDefault();
}
