namespace NetCord.Services;

public class UserId : Entity, ISpanFormattable
{
    public override ulong Id { get; }
    public User? User { get; }

    public UserId(ulong id, User? user) : this(id)
    {
        User = user;
    }

    public UserId(ulong id)
    {
        Id = id;
    }

    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
