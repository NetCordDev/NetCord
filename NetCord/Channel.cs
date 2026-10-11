using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial interface IChannel : IEntity, ISpanFormattable
{
    public ChannelType Type { get; }

    public ChannelFlags? Flags { get; }
}

public partial interface IPrivateChannel : IChannel
{
    public IReadOnlyDictionary<ulong, User> Recipients { get; }
}

public sealed partial class DMChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IPrivateChannel
{
    public ChannelType Type => ChannelType.DMChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public IReadOnlyDictionary<ulong, User> Recipients { get; } = jsonModel.Recipients!.ToDictionary(r => r.Id, r => new User(r, client));

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class GroupDMChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IPrivateChannel
{
    public ChannelType Type => ChannelType.GroupDMChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public ulong? OwnerId { get; } = jsonModel.OwnerId;

    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;

    public bool Managed { get; } = jsonModel.Managed.GetValueOrDefault();

    public IReadOnlyDictionary<ulong, User> Recipients { get; } = jsonModel.Recipients!.ToDictionary(r => r.Id, r => new User(r, client));

    public string? Name { get; } = jsonModel.Name;

    public string? IconHash { get; } = jsonModel.IconHash;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IGuildBasedChannel : IChannel
{
    public ulong GuildId { get; }
}

public partial interface IMaybeObfuscatedGuildChannel : IGuildBasedChannel
{
    public int Position { get; }

    public ulong? ParentId { get; }
}

public partial interface IGuildChannel : IMaybeObfuscatedGuildChannel
{
    public string Name { get; }

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }

    public Permissions? Permissions { get; }

    public Permissions? AppPermissions { get; }
}

public partial interface IObfuscatedGuildChannel : IMaybeObfuscatedGuildChannel;

public interface IMaybeObfuscatedTextGuildChannel : IMaybeObfuscatedGuildChannel;

public sealed partial class TextGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedTextGuildChannel, IGuildChannel
{
    public ChannelType Type => ChannelType.TextGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public string? Topic { get; } = jsonModel.Topic;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    public ThreadArchiveDuration DefaultAutoArchiveDuration { get; } = jsonModel.DefaultAutoArchiveDuration.GetValueOrDefault();

    public int? DefaultThreadSlowmodeSeconds { get; } = jsonModel.DefaultThreadSlowmodeSeconds;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedTextGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedTextGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type => ChannelType.TextGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedVoiceBasedGuildChannel : IMaybeObfuscatedGuildChannel;

public partial interface IVoiceBasedGuildChannel : IMaybeObfuscatedVoiceBasedGuildChannel, IGuildChannel
{
    public int Bitrate { get; }

    public int UserLimit { get; }

    public string? RtcRegion { get; }

    public VideoQualityMode VideoQualityMode { get; }
}

public partial interface IObfuscatedVoiceBasedGuildChannel : IMaybeObfuscatedVoiceBasedGuildChannel, IObfuscatedGuildChannel;

public partial interface IMaybeObfuscatedVoiceGuildChannel : IMaybeObfuscatedVoiceBasedGuildChannel;

public sealed partial class VoiceGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedVoiceGuildChannel, IVoiceBasedGuildChannel
{
    public ChannelType Type => ChannelType.VoiceGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public int Bitrate { get; } = jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit { get; } = jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion { get; } = jsonModel.RtcRegion;

    public VideoQualityMode VideoQualityMode { get; } = jsonModel.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedVoiceGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedVoiceGuildChannel, IObfuscatedVoiceBasedGuildChannel
{
    public ChannelType Type => ChannelType.VoiceGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedCategoryGuildChannel : IMaybeObfuscatedGuildChannel;

public sealed partial class CategoryGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedCategoryGuildChannel, IGuildChannel
{
    public ChannelType Type => ChannelType.CategoryGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    ulong? IMaybeObfuscatedGuildChannel.ParentId => null;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedCategoryGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedCategoryGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type => ChannelType.CategoryGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    ulong? IMaybeObfuscatedGuildChannel.ParentId => null;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedAnnouncementGuildChannel : IMaybeObfuscatedGuildChannel;

public sealed partial class AnnouncementGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedAnnouncementGuildChannel, IGuildChannel
{
    public ChannelType Type => ChannelType.AnnouncementGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public string? Topic { get; } = jsonModel.Topic;

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    public ThreadArchiveDuration DefaultAutoArchiveDuration { get; } = jsonModel.DefaultAutoArchiveDuration.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedAnnouncementGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedAnnouncementGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type => ChannelType.AnnouncementGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IGuildThread : IGuildBasedChannel
{
    public string Name { get; }

    public ulong ParentId { get; }

    public ulong OwnerId { get; }

    public int MessageCount { get; }

    public int UserCount { get; }

    public GuildThreadMetadata Metadata { get; }

    public ThreadCurrentUser? CurrentUser { get; }

    public int TotalMessageSent { get; }

    public ulong? LastMessageId { get; }

    public DateTimeOffset? LastPinAt { get; }

    public int? SlowmodeSeconds { get; }

    public Permissions? Permissions { get; }

    public Permissions? AppPermissions { get; }
}

public sealed partial class AnnouncementGuildThread(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IGuildThread
{
    public ChannelType Type => ChannelType.AnnouncementGuildThread;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public string Name { get; } = jsonModel.Name!;

    public ulong ParentId { get; } = jsonModel.ParentId.GetValueOrDefault();

    public ulong OwnerId { get; } = jsonModel.OwnerId.GetValueOrDefault();

    public int MessageCount { get; } = jsonModel.MessageCount.GetValueOrDefault();

    public int UserCount { get; } = jsonModel.UserCount.GetValueOrDefault();

    public GuildThreadMetadata Metadata { get; } = new GuildThreadMetadata(jsonModel.Metadata!);

    public ThreadCurrentUser? CurrentUser { get; } = jsonModel.CurrentUser is { } currentUser ? new ThreadCurrentUser(currentUser) : null;

    public int TotalMessageSent { get; } = jsonModel.TotalMessageSent.GetValueOrDefault();

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class PublicGuildThread(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IGuildThread
{
    public ChannelType Type => ChannelType.PublicGuildThread;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public string Name { get; } = jsonModel.Name!;

    public ulong ParentId { get; } = jsonModel.ParentId.GetValueOrDefault();

    public ulong OwnerId { get; } = jsonModel.OwnerId.GetValueOrDefault();

    public int MessageCount { get; } = jsonModel.MessageCount.GetValueOrDefault();

    public int UserCount { get; } = jsonModel.UserCount.GetValueOrDefault();

    public GuildThreadMetadata Metadata { get; } = new(jsonModel.Metadata!);

    public ThreadCurrentUser? CurrentUser { get; } = jsonModel.CurrentUser is { } currentUser ? new ThreadCurrentUser(currentUser) : null;

    public int TotalMessageSent { get; } = jsonModel.TotalMessageSent.GetValueOrDefault();

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyList<ulong>? AppliedTags { get; } = jsonModel.AppliedTags;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class PrivateGuildThread(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IGuildThread
{
    public ChannelType Type => ChannelType.PrivateGuildThread;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public string Name { get; } = jsonModel.Name!;

    public ulong ParentId { get; } = jsonModel.ParentId.GetValueOrDefault();

    public ulong OwnerId { get; } = jsonModel.OwnerId.GetValueOrDefault();

    public int MessageCount { get; } = jsonModel.MessageCount.GetValueOrDefault();

    public int UserCount { get; } = jsonModel.UserCount.GetValueOrDefault();

    public GuildThreadMetadata Metadata { get; } = new GuildThreadMetadata(jsonModel.Metadata!);

    public ThreadCurrentUser? CurrentUser { get; } = jsonModel.CurrentUser is { } currentUser ? new ThreadCurrentUser(currentUser) : null;

    public int TotalMessageSent { get; } = jsonModel.TotalMessageSent.GetValueOrDefault();

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedStageGuildChannel : IMaybeObfuscatedVoiceBasedGuildChannel;

public sealed partial class StageGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedStageGuildChannel, IVoiceBasedGuildChannel
{
    public ChannelType Type => ChannelType.StageGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public int Bitrate { get; } = jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit { get; } = jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion { get; } = jsonModel.RtcRegion;

    public VideoQualityMode VideoQualityMode { get; } = jsonModel.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedStageGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedStageGuildChannel, IObfuscatedVoiceBasedGuildChannel
{
    public ChannelType Type => ChannelType.StageGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedDirectoryGuildChannel : IMaybeObfuscatedGuildChannel;

public sealed partial class DirectoryGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedDirectoryGuildChannel, IGuildChannel
{
    public ChannelType Type => ChannelType.DirectoryGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedDirectoryGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedDirectoryGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type => ChannelType.DirectoryGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedThreadOnlyGuildChannel : IMaybeObfuscatedGuildChannel;

public partial interface IThreadOnlyGuildChannel : IMaybeObfuscatedThreadOnlyGuildChannel, IGuildChannel
{
    public bool Nsfw { get; }

    public string? Topic { get; }

    public ulong? LastMessageId { get; }

    public DateTimeOffset? LastPinAt { get; }

    public int? SlowmodeSeconds { get; }

    public ThreadArchiveDuration? DefaultAutoArchiveDuration { get; }

    public IReadOnlyList<ForumTag>? AvailableTags { get; }

    public ForumGuildChannelDefaultReaction? DefaultReactionEmoji { get; }

    public int? DefaultThreadSlowmodeSeconds { get; }

    public SortOrderType? DefaultSortOrder { get; }

    public ForumLayoutType? DefaultForumLayout { get; }
}

public partial interface IObfuscatedThreadOnlyGuildChannel : IMaybeObfuscatedThreadOnlyGuildChannel, IObfuscatedGuildChannel;

public partial interface IMaybeObfuscatedForumGuildChannel : IMaybeObfuscatedThreadOnlyGuildChannel;

public sealed partial class ForumGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedForumGuildChannel, IThreadOnlyGuildChannel
{
    public ChannelType Type => ChannelType.ForumGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    public string? Topic { get; } = jsonModel.Topic;

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public ThreadArchiveDuration? DefaultAutoArchiveDuration { get; } = jsonModel.DefaultAutoArchiveDuration;

    public IReadOnlyList<ForumTag>? AvailableTags { get; } = [.. jsonModel.AvailableTags!.Select(t => new ForumTag(t))];

    public ForumGuildChannelDefaultReaction? DefaultReactionEmoji { get; } = jsonModel.DefaultReactionEmoji is { } defaultReactionEmoji ? new ForumGuildChannelDefaultReaction(defaultReactionEmoji) : null;

    public int? DefaultThreadSlowmodeSeconds { get; } = jsonModel.DefaultThreadSlowmodeSeconds;

    public SortOrderType? DefaultSortOrder { get; } = jsonModel.DefaultSortOrder;

    public ForumLayoutType? DefaultForumLayout { get; } = jsonModel.DefaultForumLayout;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedForumGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedForumGuildChannel, IObfuscatedThreadOnlyGuildChannel
{
    public ChannelType Type => ChannelType.ForumGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedMediaGuildChannel : IMaybeObfuscatedThreadOnlyGuildChannel;

public sealed partial class MediaGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedMediaGuildChannel, IThreadOnlyGuildChannel
{
    public ChannelType Type => ChannelType.MediaGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    public string? Topic { get; } = jsonModel.Topic;

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public ThreadArchiveDuration? DefaultAutoArchiveDuration { get; } = jsonModel.DefaultAutoArchiveDuration;

    public IReadOnlyList<ForumTag>? AvailableTags { get; } = [.. jsonModel.AvailableTags!.Select(t => new ForumTag(t))];

    public ForumGuildChannelDefaultReaction? DefaultReactionEmoji { get; } = jsonModel.DefaultReactionEmoji is { } defaultReactionEmoji ? new ForumGuildChannelDefaultReaction(defaultReactionEmoji) : null;

    public int? DefaultThreadSlowmodeSeconds { get; } = jsonModel.DefaultThreadSlowmodeSeconds;

    public SortOrderType? DefaultSortOrder { get; } = jsonModel.DefaultSortOrder;

    public ForumLayoutType? DefaultForumLayout { get; } = jsonModel.DefaultForumLayout;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedMediaGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedMediaGuildChannel, IObfuscatedThreadOnlyGuildChannel
{
    public ChannelType Type => ChannelType.MediaGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedApplicationGuildChannel : IMaybeObfuscatedGuildChannel;

public sealed partial class ApplicationGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedApplicationGuildChannel, IGuildChannel
{
    public ChannelType Type => ChannelType.ApplicationGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public string Name { get; } = jsonModel.Name!;

    public Permissions? Permissions { get; } = jsonModel.Permissions;

    public Permissions? AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites
        ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p))
        : [];

    public ulong? LastMessageId { get; } = jsonModel.LastMessageId;

    public DateTimeOffset? LastPinAt { get; } = jsonModel.LastPinAt;

    public string? Topic { get; } = jsonModel.Topic;

    public int? SlowmodeSeconds { get; } = jsonModel.SlowmodeSeconds;

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

    public ThreadArchiveDuration DefaultAutoArchiveDuration { get; } = jsonModel.DefaultAutoArchiveDuration.GetValueOrDefault();

    public int? DefaultThreadSlowmodeSeconds { get; } = jsonModel.DefaultThreadSlowmodeSeconds;

    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedApplicationGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedApplicationGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type => ChannelType.ApplicationGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}
