using NetCord;
using NetCord.Rest;

#pragma warning disable IDE0017, IDE0059

_ = CreateMessage<MessageProperties>();

UseCreateMessage();

_ = PropertiesAsync();

ImplicitConversion();

static T CreateMessage<T>() where T : IMessageProperties, new()
{
    return new()
    {
        Content = "Hello, World!",
        Components = [],
    };
}

static void UseCreateMessage()
{
    var message = CreateMessage<MessageProperties>();

    var interactionMessage = CreateMessage<InteractionMessageProperties>();
}

async static Task PropertiesAsync()
{
    IMessageProperties message = null!;

    message.Content = "Hello, World!";

    EmbedProperties embed;
    
    embed = new()
    {
        
    };

    message.Embeds = [embed];

    message.AllowedMentions = AllowedMentionsProperties.All;

    message.AllowedMentions = AllowedMentionsProperties.None;

    message.AllowedMentions = new()
    {
        Everyone = true, // Allow @everyone and @here
        ReplyMention = true, // Allow reply mention
        AllowedRoles = [988888771187581010], // Allow specific roles
        AllowedUsers = [265546281693347841], // Allow specific users
    };

    AttachmentProperties attachment;

    attachment = new AttachmentProperties("hello.txt", new MemoryStream("Hello!"u8.ToArray()));

    attachment = new Base64AttachmentProperties("hello.txt", new MemoryStream("SGVsbG8sIGJhc2U2NCE="u8.ToArray()));

    attachment = new QuotedPrintableAttachmentProperties("polish.txt",
                                                         new MemoryStream("R=C3=B3=C5=BCowy is pink"u8.ToArray()));

    TextChannel textChannel = null!;

    HttpClient httpClient = null!;

    // Create a bucket with a file named "hello.txt"
    var buckets = await textChannel.CreateGoogleCloudPlatformStorageBucketsAsync([new("hello.txt")]);

    var bucket = buckets[0];

    // Upload the file content to the bucket
    var response = await httpClient.PutAsync(bucket.UploadUrl, new StringContent("Hello, Google!"));
    response.EnsureSuccessStatusCode();

    attachment = new GoogleCloudPlatformAttachmentProperties("hello.txt", bucket.UploadFileName);

    attachment.Title = "Hello, World!";

    attachment.Description = "This is a file named hello.txt";

    message.Attachments = [attachment];

    message.Components =
    [
    ];

    message.Flags = MessageFlags.SuppressEmbeds | MessageFlags.SuppressNotifications;
}

static void ImplicitConversion()
{
    MessageProperties message = "Hello, World!";
}
