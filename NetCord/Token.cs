using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public class Token
{
    public Token(TokenType type, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));

        Type = type;
        RawToken = token;
    }

    public TokenType Type { get; }

    public string RawToken { get; }

    public ulong Id
    {
        get
        {
            int index = RawToken.IndexOf('.');
            if (index != -1)
            {
                var idBase64 = RawToken[..index];
                var idConverted = Convert.FromBase64String(idBase64.PadRight((idBase64.Length + 3) / 4 * 4, '='));
                if (Utf8Parser.TryParse(idConverted, out ulong id, out int bytesConsumed) && idConverted.Length == bytesConsumed)
                    return id;
            }

            throw new InvalidOperationException("Invalid token provided.");
        }
    }

    public string ToHttpHeader() => Type switch
    {
        TokenType.Bot => $"Bot {RawToken}",
        TokenType.Bearer => $"Bearer {RawToken}",
        _ => RawToken,
    };

    public static bool operator ==(Token? left, Token? right) => left is null ? right is null : right is not null && left.Type == right.Type && left.RawToken == right.RawToken;

    public static bool operator !=(Token? left, Token? right) => !(left == right);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Token token && Type == token.Type && RawToken == token.RawToken;

    public override int GetHashCode() => RawToken.GetHashCode();

    public override string ToString() => RawToken;
}
