namespace NetCord.Services;

/// <summary>
/// Represents a user ID with an optional user object.
/// </summary>
/// <param name="id"><inheritdoc cref="Id" path="/summary" /></param>
public class UserId(ulong id) : Entity, ISpanFormattable
{
    /// <summary>
    /// The ID of the user.
    /// </summary>
    public override ulong Id { get; } = id;

    /// <summary>
    /// The user object, if available.
    /// </summary>
    public User? User { get; }

    /// <summary>
    /// <inheritdoc cref="UserId" path="/summary" />
    /// </summary>
    /// <param name="id"><inheritdoc cref="Id" path="/summary" /></param>
    /// <param name="user"><inheritdoc cref="User" path="/summary" /></param>
    public UserId(ulong id, User? user) : this(id)
    {
        User = user;
    }

    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
