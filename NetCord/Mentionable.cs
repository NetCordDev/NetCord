namespace NetCord;

public abstract class Mentionable : Entity
{
    private Mentionable()
    {
    }

    public abstract override string ToString();

    public abstract override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null);

    public sealed class User(NetCord.User value) : Mentionable
    {
        public NetCord.User Value => value;

        public override ulong Id => value.Id;

        public override string ToString() => value.ToString();

        public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => value.TryFormat(destination, out charsWritten, format, provider);
    }

    public sealed class Role(NetCord.Role value) : Mentionable
    {
        public NetCord.Role Value => value;

        public override ulong Id => Value.Id;

        public override string ToString() => value.ToString();

        public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => value.TryFormat(destination, out charsWritten, format, provider);
    }
}
