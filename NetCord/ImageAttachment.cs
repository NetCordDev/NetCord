namespace NetCord;

public class ImageAttachment(JsonModels.JsonAttachment jsonModel) : Attachment(jsonModel)
{
    /// <summary>
    /// Height of file.
    /// </summary>
    public int Height => _jsonModel.Height.GetValueOrDefault();

    /// <summary>
    /// Width of file.
    /// </summary>
    public int Width => _jsonModel.Width.GetValueOrDefault();
}
