namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial record GuildMessagesSearchPaginationProperties : PaginationProperties<int>, IPaginationProperties<int, GuildMessagesSearchPaginationProperties>
{
    public GuildMessagesSearchSortingMode? SortBy { get; set; }

    public string? Content { get; set; }

    public int? Slop { get; set; }

    public IEnumerable<string>? Contents { get; set; }

    public IEnumerable<ulong>? AuthorIds { get; set; }

    public GuildMessagesSearchAuthorTypes? AuthorTypes { get; set; }

    public IEnumerable<ulong>? Mentions { get; set; }

    public bool? MentionEveryone { get; set; }

    public ulong? MinId { get; set; }

    public ulong? MaxId { get; set; }

    public GuildMessagesSearchHasOptions? Has { get; set; }

    public IEnumerable<string>? LinkHostnames { get; set; }

    public IEnumerable<string>? EmbedProviders { get; set; }

    public GuildMessagesSearchEmbedTypes? EmbedTypes { get; set; }

    public IEnumerable<string>? AttachmentExtensions { get; set; }

    public IEnumerable<string>? AttachmentFilenames { get; set; }

    public bool? Pinned { get; set; }

    public ulong? CommandId { get; set; }

    public string? CommandName { get; set; }

    public bool? IncludeNsfw { get; set; }

    public IEnumerable<ulong>? ChannelIds { get; set; }

    public static GuildMessagesSearchPaginationProperties Create() => new();
    public static GuildMessagesSearchPaginationProperties Create(GuildMessagesSearchPaginationProperties properties) => new(properties);
}

public enum GuildMessagesSearchSortingMode : byte
{
    Relevance,
    Timestamp,
}

[Flags]
public enum GuildMessagesSearchAuthorTypes : byte
{
    User = 1 << 0,
    Bot = 1 << 1,
    Webhook = 1 << 2,
    NoUser = 1 << 3,
    NoBot = 1 << 4,
    NoWebhook = 1 << 5,
}

[Flags]
public enum GuildMessagesSearchHasOptions : uint
{
    Link = 1 << 0,
    Embed = 1 << 1,
    File = 1 << 2,
    Image = 1 << 3,
    Video = 1 << 4,
    Sound = 1 << 5,
    Sticker = 1 << 6,
    Poll = 1 << 7,
    Snapshot = 1 << 8,
    NoLink = 1 << 9,
    NoEmbed = 1 << 10,
    NoFile = 1 << 11,
    NoImage = 1 << 12,
    NoVideo = 1 << 13,
    NoSound = 1 << 14,
    NoSticker = 1 << 15,
    NoPoll = 1 << 16,
    NoSnapshot = 1 << 17,
}

[Flags]
public enum GuildMessagesSearchEmbedTypes : byte
{
    Image = 1 << 0,
    Video = 1 << 1,
    Gifv = 1 << 2,
    Sound = 1 << 3,
    Article = 1 << 4,
}
