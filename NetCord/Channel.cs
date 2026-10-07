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

    public IReadOnlyDictionary<ulong, User> Recipients { get; } = jsonModel.Recipients!.ToDictionary(r => r.Id, r => new User(r, client));

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class GroupDMChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IPrivateChannel
{
    public ChannelType Type => ChannelType.GroupDMChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public IReadOnlyDictionary<ulong, User> Recipients { get; } = jsonModel.Recipients!.ToDictionary(r => r.Id, r => new User(r, client));

    public string Name { get; } = jsonModel.Name!;

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

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

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

    public bool Nsfw { get; } = jsonModel.Nsfw.GetValueOrDefault();

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

public partial interface IMaybeObfuscatedForumGuildChannel : IMaybeObfuscatedGuildChannel;

public sealed partial class ForumGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedForumGuildChannel, IGuildChannel
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

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedForumGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedForumGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type => ChannelType.ForumGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IMaybeObfuscatedMediaGuildChannel : IMaybeObfuscatedGuildChannel;

public sealed partial class MediaGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedMediaGuildChannel, IGuildChannel
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

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedMediaGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedMediaGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type => ChannelType.MediaGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}
