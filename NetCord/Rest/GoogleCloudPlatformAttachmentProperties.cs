namespace NetCord.Rest;

public partial class GoogleCloudPlatformAttachmentProperties : AttachmentProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="uploadedFileName">Name of the upload.</param>
    public GoogleCloudPlatformAttachmentProperties(string fileName, string uploadedFileName) : base(fileName)
    {
        UploadedFileName = uploadedFileName;
    }

    /// <summary>
    /// Name of the upload.
    /// </summary>
    public string UploadedFileName { get; set; }
}
