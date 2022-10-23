using System.Buffers.Text;

namespace NetCord;

public class Token
{
    public Token(TokenType type, string token)
    {
        ArgumentNullException.ThrowIfNull(type);
        Type = type;
        RawToken = token;
    }

    public TokenType Type { get; }
    public string RawToken { get; }

    public string ToHttpHeader() => Type == TokenType.Bot ? $"Bot {RawToken}" : RawToken;

    public ulong Id
    {
        get
        {
            var idBase64 = RawToken[..RawToken.IndexOf('.')];
            var idConverted = Convert.FromBase64String(idBase64.PadRight((idBase64.Length + 3) / 4 * 4, '='));
            if (Utf8Parser.TryParse(idConverted, out ulong id, out int bytesConsumed) && idConverted.Length == bytesConsumed)
                return id;
            throw new("Invalid token provided.");
        }
    }
}
