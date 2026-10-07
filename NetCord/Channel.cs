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

public partial interface ICategorizableGuildChannel : IMaybeObfuscatedGuildChannel
{
    public string Name { get; }

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
}

public partial interface IObfuscatedGuildChannel : IMaybeObfuscatedGuildChannel;

public partial interface IGuildChannel : IMaybeObfuscatedGuildChannel
{
    public string Name { get; }

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
}

public sealed partial class TextGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IGuildChannel
{
    public ChannelType Type => ChannelType.TextGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId => jsonModel.ParentId;

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    public string Name => jsonModel.Name!;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p)) : [];

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedTextGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type { get; } = ChannelType.TextGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public partial interface IVoiceGuildChannel : IGuildChannel
{
    public int Bitrate { get; }

    public int UserLimit { get; }

    public string? RtcRegion { get; }
}

public partial interface IObfuscatedVoiceGuildChannel : IMaybeObfuscatedGuildChannel, IObfuscatedGuildChannel;

public sealed partial class VoiceGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IVoiceGuildChannel
{
    public ChannelType Type => ChannelType.VoiceGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId => jsonModel.ParentId;

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    public string Name => jsonModel.Name!;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p)) : [];

    public int Bitrate { get; } = jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit { get; } = jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion { get; } = jsonModel.RtcRegion;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedVoiceGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IObfuscatedVoiceGuildChannel
{
    public ChannelType Type { get; } = ChannelType.VoiceGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class CategoryGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IGuildChannel
{
    public ChannelType Type => ChannelType.CategoryGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    ulong? IMaybeObfuscatedGuildChannel.ParentId => null;

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    public string Name => jsonModel.Name!;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p)) : [];

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedCategoryGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type { get; } = ChannelType.CategoryGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId => null;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class AnnouncementGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IGuildChannel
{
    public ChannelType Type => ChannelType.AnnouncementGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId => jsonModel.ParentId;

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    public string Name => jsonModel.Name!;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p)) : [];

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedAnnouncementGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IMaybeObfuscatedGuildChannel, IObfuscatedGuildChannel
{
    public ChannelType Type { get; } = ChannelType.AnnouncementGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId { get; } = jsonModel.ParentId;

    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class StageGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IVoiceGuildChannel
{
    public ChannelType Type => ChannelType.StageGuildChannel;

    public ChannelFlags? Flags { get; } = jsonModel.Flags;

    public int Position { get; } = jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId => jsonModel.ParentId;

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    public string Name => jsonModel.Name!;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites is { } permissionOverwrites ? permissionOverwrites.ToDictionary(p => p.Id, p => new PermissionOverwrite(p)) : [];

    public int Bitrate { get; } = jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit { get; } = jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion { get; } = jsonModel.RtcRegion;

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}

public sealed partial class ObfuscatedStageGuildChannel(JsonChannel jsonModel, RestClient client) : ClientEntity(jsonModel, client), IObfuscatedVoiceGuildChannel
{
    public ChannelType Type { get; } = ChannelType.StageGuildChannel;

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

public class GuildThreadMetadata(JsonGuildThreadMetadata jsonModel)
{
    public bool Archived { get; } = jsonModel.Archived;

    public ThreadArchiveDuration AutoArchiveDuration { get; } = jsonModel.AutoArchiveDuration;

    public DateTimeOffset ArchiveTimestamp { get; } = jsonModel.ArchiveTimestamp;

    public bool Locked { get; } = jsonModel.Locked;

    public bool? Invitable { get; } = jsonModel.Invitable;

    public DateTimeOffset? CreateTimestamp { get; } = jsonModel.CreateTimestamp;
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

    public GuildThreadMetadata Metadata { get; } = new GuildThreadMetadata(jsonModel.Metadata!);

    public ThreadCurrentUser? CurrentUser { get; } = jsonModel.CurrentUser is not null ? new ThreadCurrentUser(jsonModel.CurrentUser) : null;

    public int TotalMessageSent { get; } = jsonModel.TotalMessageSent.GetValueOrDefault();

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

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

    public ThreadCurrentUser? CurrentUser { get; } = jsonModel.CurrentUser is not null ? new ThreadCurrentUser(jsonModel.CurrentUser) : null;

    public int TotalMessageSent { get; } = jsonModel.TotalMessageSent.GetValueOrDefault();

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
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

    public ThreadCurrentUser? CurrentUser { get; } = jsonModel.CurrentUser is not null ? new ThreadCurrentUser(jsonModel.CurrentUser) : null;

    public int TotalMessageSent { get; } = jsonModel.TotalMessageSent.GetValueOrDefault();

    public ulong GuildId => jsonModel.GuildId.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}
