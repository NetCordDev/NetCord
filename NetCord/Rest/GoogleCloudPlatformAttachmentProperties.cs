namespace NetCord.Rest;

public partial class GoogleCloudPlatformAttachmentProperties : AttachmentProperties
{
    public GoogleCloudPlatformAttachmentProperties(string fileName, string uploadedFileName) : base(fileName)
    {
        UploadedFileName = uploadedFileName;
    }

    public string UploadedFileName { get; }
}
