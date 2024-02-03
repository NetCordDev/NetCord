using System.Runtime.CompilerServices;

namespace NetCord;

public class BotToken : IEntityToken
{
    public BotToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));

        Id = IEntityToken.TryGetTokenId(token, out var id)
            ? id
            : throw new ArgumentException($"'{nameof(token)}' is not a valid bot token.", nameof(token));

        RawToken = token;
    }

    public string RawToken { get; }

    public string HttpHeaderValue => $"Bot {RawToken}";

    public ulong Id { get; }

    public DateTimeOffset CreatedAt => Snowflake.CreatedAt(Id);
}

public class BearerToken : IToken
{
    public BearerToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));

        RawToken = token;
    }

    public string RawToken { get; }

    public string HttpHeaderValue => $"Bearer {RawToken}";
}

public interface IEntityToken : IToken, IEntity
{
    [SkipLocalsInit]
    protected static bool TryGetTokenId(ReadOnlySpan<char> token, out ulong id)
    {
        const int MaxSnowflakeLength = 19;
        const int MaxBase64Length = ((MaxSnowflakeLength * 4) + 2) / 3;
        const int MaxBase64LengthWithPadding = (MaxSnowflakeLength + 2) * 4 / 3;

        int index = token.IndexOf('.');

        if (index is >= 0 and <= MaxBase64Length)
        {
            var chars = (stackalloc char[MaxBase64LengthWithPadding])[..((index + 3) / 4 * 4)];
            token[..index].CopyTo(chars);
            chars[index..].Fill('=');

            Span<byte> bytes = stackalloc byte[MaxSnowflakeLength];

            if (Convert.TryFromBase64Chars(chars, bytes, out int bytesWritten) && Snowflake.TryParse(bytes[..bytesWritten], out id))
                return true;
        }

        id = default;
        return false;
    }
}

public interface IToken
{
    public string RawToken { get; }

    public string HttpHeaderValue { get; }
}
