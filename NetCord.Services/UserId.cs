namespace NetCord.Services;

public class UserId(ulong id) : Entity, ISpanFormattable
{
    public override ulong Id { get; } = id;
    public User? User { get; }

    public UserId(ulong id, User? user) : this(id)
    {
        User = user;
    }

    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
