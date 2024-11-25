using NetCord;
using NetCord.Rest;

#pragma warning disable IDE0017, IDE0040, IDE0051, IDE0059, CS8321

async static Task PropertiesAsync()
{
    IMessageProperties message = null!;

    message.Content = "Hello, World!";

    message.WithContent("Hello, World!");

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

    embed = new EmbedProperties()
        .WithTitle("Welcome to the Baking Club!")
        .WithDescription("Join us for delicious recipes and baking tips!")
        .WithUrl("https://example.com")
        .WithTimestamp(DateTimeOffset.UtcNow)
        .WithColor(new(0xFFA500))
        .WithFooter(new EmbedFooterProperties()
            .WithText("Happy Baking!")
            .WithIconUrl("https://example.com/images/baking-icon.png"))
        .WithImage("https://example/com/images/cake.jpg")
        .WithThumbnail("https://example.com/images/rolling-pin.png")
        .WithAuthor(new EmbedAuthorProperties()
            .WithName("Baking Club")
            .WithUrl("https://example.com")
            .WithIconUrl("https://example.com/images/club-logo.png"))
        .AddFields(
            new EmbedFieldProperties()
                .WithName("Today's Special Recipe")
                .WithValue("Chocolate Lava Cake"),
            new EmbedFieldProperties()
                .WithName("Next Meetup")
                .WithValue("Sunday, 4 PM")
                .WithInline(),
            new EmbedFieldProperties()
                .WithName("Location")
                .WithValue("123 Baker's Street")
                .WithInline(),
            new EmbedFieldProperties()
                .WithName("Membership Fee")
                .WithValue("Free for the first month!")
                .WithInline());

    message.Embeds = [embed];

    message.AddEmbeds(embed);

    message.AllowedMentions = AllowedMentionsProperties.All;

    message.WithAllowedMentions(AllowedMentionsProperties.All);

    message.AllowedMentions = AllowedMentionsProperties.None;

    message.WithAllowedMentions(AllowedMentionsProperties.None);

    message.AllowedMentions = new()
    {
        Everyone = true, // Allow @everyone and @here
        ReplyMention = true, // Allow reply mention
        AllowedRoles = [988888771187581010], // Allow specific roles
        AllowedUsers = [265546281693347841], // Allow specific users
    };

    message.WithAllowedMentions(new AllowedMentionsProperties()
        .WithEveryone() // Allow @everyone and @here
        .WithReplyMention() // Allow reply mention
        .AddAllowedRoles(988888771187581010) // Allow specific roles
        .AddAllowedUsers(265546281693347841)); // Allow specific users

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

    attachment
        .WithTitle("Hello, World!")
        .WithDescription("This is a file named hello.txt");

    message.Attachments = [attachment];

    message.AddAttachments(attachment);

    ComponentProperties component;

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

    component = new ActionRowProperties()
        .AddButtons(
            new ButtonProperties("welcome", "Welcome", new("👋"), ButtonStyle.Primary),
            new ButtonProperties("hug", new EmojiProperties(356377264209920002), ButtonStyle.Success),
            new ButtonProperties("goodbye", "Goodbye", ButtonStyle.Secondary)
                .WithDisabled(),
            new LinkButtonProperties("https://netcord.dev", "Learn More"),
            new PremiumButtonProperties(1271914991536312372));

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

    component = new StringMenuProperties("animal")
        .AddOptions(
            new StringMenuSelectOptionProperties("Dog", "dog")
                .WithDefault()
                .WithEmoji(new("🐶"))
                .WithDescription("A loyal companion"),
            new StringMenuSelectOptionProperties("Cat", "cat")
                .WithEmoji(new("🐱"))
                .WithDescription("A curious feline"),
            new StringMenuSelectOptionProperties("Bird", "bird")
                .WithEmoji(new("🐦"))
                .WithDescription("A chirpy flyer"));

    component = new ChannelMenuProperties("channel")
    {
        DefaultValues = [1124777547687788626],
        ChannelTypes = [ChannelType.ForumGuildChannel, ChannelType.PublicGuildThread],
    };

    component = new ChannelMenuProperties("channel")
        .AddDefaultValues(1124777547687788626)
        .AddChannelTypes(ChannelType.ForumGuildChannel, ChannelType.PublicGuildThread);

    component = new MentionableMenuProperties("mentionable")
    {
        DefaultValues =
        [
            new(803324257194082314, MentionableValueType.User),
        ],
    };

    component = new MentionableMenuProperties("mentionable")
        .AddDefaultValues(
            new MentionableValueProperties(803324257194082314, MentionableValueType.User));

    component = new RoleMenuProperties("role")
    {
        DefaultValues = [803169206115237908],
    };

    component = new RoleMenuProperties("role")
        .AddDefaultValues(803169206115237908);

    component = new UserMenuProperties("user")
    {
        DefaultValues = [233590074724319233],
    };

    component = new UserMenuProperties("user")
        .AddDefaultValues(233590074724319233);

    message.Components = [component];

    message.AddComponents(component);

    message.Flags = MessageFlags.SuppressEmbeds | MessageFlags.SuppressNotifications;

    message.WithFlags(MessageFlags.SuppressEmbeds | MessageFlags.SuppressNotifications);
}

static void Menu()
{
    MenuProperties component = null!;

    component.Placeholder = "Select 2-5 animals";
    component.MinValues = 2;
    component.MaxValues = 5;
    component.Disabled = true;

    component
        .WithPlaceholder("Select 2-5 animals")
        .WithMinValues(2)
        .WithMaxValues(5)
        .WithDisabled();
}

static void ImplicitConversion()
{
    MessageProperties message = "Hello, World!";
}

static class Classic
{
    static T CreateMessage<T>() where T : IMessageProperties, new()
    {
        return new()
        {
            Content = "Hello, World!",
            Components = [],
        };
    }
}

static class Fluent
{
    static T CreateMessage<T>() where T : IMessageProperties, new()
    {
        T message = new();

        message
            .WithContent("Hello, World!")
            .WithComponents([]);

        return message;
    }

    static void UseCreateMessage()
    {
        IMessageProperties message;

        message = CreateMessage<MessageProperties>();

        message = CreateMessage<InteractionMessageProperties>();
    }
}
