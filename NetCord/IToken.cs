using System.Runtime.CompilerServices;

namespace NetCord;

/// <summary>
/// Represents a Discord bot token.
/// </summary>
public class BotToken : IEntityToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BotToken"/> class.
    /// </summary>
    /// <param name="token">The raw bot token.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="token"/> is null or empty, or is not a valid bot token.
    /// </exception>
    public BotToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));

        Id = IEntityToken.TryGetTokenId(token, out var id)
            ? id
            : throw new ArgumentException($"'{nameof(token)}' is not a valid bot token.", nameof(token));

        RawToken = token;
    }

    /// <summary>
    /// Gets the raw bot token.
    /// </summary>
    public string RawToken { get; }

    /// <summary>
    /// Gets the value to use for the HTTP <c>Authorization</c> header.
    /// </summary>
    /// <remarks>
    /// The value uses the <c>Bot</c> authentication scheme.
    /// </remarks>
    public string HttpHeaderValue => $"Bot {RawToken}";

    /// <summary>
    /// Gets the entity ID encoded in the token.
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    /// Gets the creation time derived from <see cref="Id"/>.
    /// </summary>
    public DateTimeOffset CreatedAt => Snowflake.Timestamp(Id);
}

/// <summary>
/// Represents an OAuth2 bearer token.
/// </summary>
public class BearerToken : IToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BearerToken"/> class.
    /// </summary>
    /// <param name="token">The raw OAuth2 bearer token.</param>
    /// <exception cref="ArgumentException"><paramref name="token"/> is null or empty.</exception>
    public BearerToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));

        RawToken = token;
    }

    /// <summary>
    /// Gets the raw bearer token.
    /// </summary>
    public string RawToken { get; }

    /// <summary>
    /// Gets the value to use for the HTTP <c>Authorization</c> header.
    /// </summary>
    /// <remarks>
    /// The value uses the <c>Bearer</c> authentication scheme.
    /// </remarks>
    public string HttpHeaderValue => $"Bearer {RawToken}";
}

/// <summary>
/// Represents an authentication token that identifies a Discord entity.
/// </summary>
public interface IEntityToken : IToken, IEntity
{
    /// <summary>
    /// Attempts to extract the entity ID encoded in a token.
    /// </summary>
    /// <param name="token">The token to inspect.</param>
    /// <param name="id">When this method returns, contains the decoded entity ID if successful.</param>
    /// <returns><see langword="true"/> if the entity ID was decoded; otherwise, <see langword="false"/>.</returns>
    [SkipLocalsInit]
    protected static bool TryGetTokenId(ReadOnlySpan<char> token, out ulong id)
    {
        const int MaxSnowflakeLength = 20;
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

/// <summary>
/// Represents a Discord authentication token.
/// </summary>
public interface IToken
{
    /// <summary>
    /// Gets the raw token.
    /// </summary>
    public string RawToken { get; }

    /// <summary>
    /// Gets the value to use for the HTTP <c>Authorization</c> header.
    /// </summary>
    public string HttpHeaderValue { get; }
}
