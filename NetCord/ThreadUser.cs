using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a user that has joined a thread.
/// </summary>
public class ThreadUser(JsonThreadUser jsonModel, RestClient client) : ClientEntity(client), ISpanFormattable
{
    /// <summary>
    /// The thread user's ID.
    /// </summary>
    public override ulong Id { get; } = jsonModel.UserId;

    /// <summary>
    /// The ID corresponding to the joined thread.
    /// </summary>
    public ulong ThreadId { get; } = jsonModel.ThreadId;

    /// <summary>
    /// The timestamp at which the user last joined the thread.
    /// </summary>
    public DateTimeOffset JoinTimestamp { get; } = jsonModel.JoinTimestamp;

    /// <summary>
    /// The user's thread settings, currently only used for notifications.
    /// </summary>
    public ThreadUserFlags Flags { get; } = jsonModel.Flags;

    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
