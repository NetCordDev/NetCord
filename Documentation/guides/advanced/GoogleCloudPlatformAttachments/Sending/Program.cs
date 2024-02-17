using NetCord;
using NetCord.Rest;

using RestClient client = new(new BotToken("Token from Discord Developer Portal"));

ulong channelId = 864636357821726730; // Note that it doesn't need to be the same channel id as channel id used to upload
var fileName = "file.txt";
var uploadFileName = "cc7c13c1-a13d-4b3a-b978-e2c003466155/file.txt"; // It's returned when creating Google Cloud Platform Storage Bucket

await client.SendMessageAsync(channelId, new()
{
    Attachments = [new GoogleCloudPlatformAttachmentProperties(fileName, uploadFileName)]
});
