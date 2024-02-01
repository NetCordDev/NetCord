using System.Runtime.CompilerServices;

namespace NetCord;

public class Token : IEntity
{
    public Token(TokenType type, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));

        Id = GetId(token);
        Type = type;
        RawToken = token;
    }

    public ulong Id { get; }

    public TokenType Type { get; }

    public string RawToken { get; }

    public DateTimeOffset CreatedAt => Snowflake.CreatedAt(Id);

    [SkipLocalsInit]
    public static ulong GetId(string token)
    {
        const int MaxSnowflakeLength = 19;
        const int MaxBase64Length = ((MaxSnowflakeLength * 4) + 2) / 3;
        const int MaxBase64LengthWithPadding = (MaxSnowflakeLength + 2) * 4 / 3;

        int index = token.IndexOf('.');

        if (index is >= 0 and <= MaxBase64Length)
        {
            var chars = (stackalloc char[MaxBase64LengthWithPadding])[..((index + 3) / 4 * 4)];
            token.AsSpan(0, index).CopyTo(chars);
            chars[index..].Fill('=');

            Span<byte> bytes = stackalloc byte[MaxSnowflakeLength];

            if (Convert.TryFromBase64Chars(chars, bytes, out int bytesWritten) && Snowflake.TryParse(bytes[..bytesWritten], out ulong id))
                return id;
        }

        throw new ArgumentException("Invalid token provided.", nameof(token));
    }

    public string ToHttpHeader() => Type switch
    {
        TokenType.Bot => $"Bot {RawToken}",
        TokenType.Bearer => $"Bearer {RawToken}",
        _ => RawToken,
    };

    public override string ToString() => RawToken;
}
