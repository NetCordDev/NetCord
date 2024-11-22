using NetCord;
using NetCord.Rest;

#pragma warning disable IDE0017, IDE0059, CS8321

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
    IMessageProperties message;

    message = CreateMessage<MessageProperties>();

    message = CreateMessage<InteractionMessageProperties>();
}

async static Task PropertiesAsync()
{
    IMessageProperties message = null!;

    message.Content = "Hello, World!";

    EmbedProperties embed;

    embed = new()
    {
        Title = "Welcome to the Baking Club!",
        Description = "Join us for delicious recipes and baking tips!",
        Url = "https://example.com",
        Timestamp = DateTimeOffset.UtcNow,
        Color = new(0xFFA500),
        Footer = new()
        {
            Text = "Happy Baking!",
            IconUrl = "https://example.com/images/baking-icon.png",
        },
        Image = "https://example.com/images/cake.jpg",
        Thumbnail = "https://example.com/images/rolling-pin.png",
        Author = new()
        {
            Name = "Baking Club",
            Url = "https://example.com",
            IconUrl = "https://example.com/images/club-logo.png",
        },
        Fields =
        [
            new()
            {
                Name = "Today's Special Recipe",
                Value = "Chocolate Lava Cake",
            },
            new()
            {
                Name = "Next Meetup",
                Value = "Sunday, 4 PM",
                Inline = true,
            },
            new()
            {
                Name = "Location",
                Value = "123 Baker's Street",
                Inline = true,
            },
            new()
            {
                Name = "Membership Fee",
                Value = "Free for the first month!",
                Inline = true,
            },
        ],
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
                                                         new MemoryStream("R=C3=B3=C5=BCowy means pink"u8.ToArray()));

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

    MessageComponentProperties component;

    component = new ActionRowProperties
    {
        new ButtonProperties("welcome", "Welcome", new("👋"), ButtonStyle.Primary),
        new ButtonProperties("hug", new EmojiProperties(356377264209920002), ButtonStyle.Success),
        new ButtonProperties("goodbye", "Goodbye", ButtonStyle.Secondary)
        {
            Disabled = true,
        },
        new LinkButtonProperties("https://netcord.dev", "Learn More"),
        new PremiumButtonProperties(1271914991536312372),
    };

    component = new StringMenuProperties("animal")
    {
        new("Dog", "dog")
        {
            Default = true,
            Emoji = new("🐶"),
            Description = "A loyal companion",
        },
        new("Cat", "cat")
        {
            Emoji = new("🐱"),
            Description = "A curious feline",
        },
        new("Bird", "bird")
        {
            Emoji = new("🐦"),
            Description = "A chirpy flyer",
        },
    };

    component = new ChannelMenuProperties("channel")
    {
        DefaultValues = [1124777547687788626],
        ChannelTypes = [ChannelType.ForumGuildChannel, ChannelType.PublicGuildThread],
    };

    component = new MentionableMenuProperties("mentionable")
    {
        DefaultValues =
        [
            new(803324257194082314, MentionableValueType.User),
        ],
    };

    component = new RoleMenuProperties("role")
    {
        DefaultValues = [803169206115237908],
    };

    component = new UserMenuProperties("user")
    {
        DefaultValues = [233590074724319233],
    };

    message.Components = [component];

    message.Flags = MessageFlags.SuppressEmbeds | MessageFlags.SuppressNotifications;
}

static void Menu()
{
    MenuProperties component = null!;

    component.Placeholder = "Select 2-5 animals";
    component.MinValues = 2;
    component.MaxValues = 5;
    component.Disabled = true;
}

static void ImplicitConversion()
{
    MessageProperties message = "Hello, World!";
}
