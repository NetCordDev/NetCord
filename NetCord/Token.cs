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
            if (Utf8Parser.TryParse(Convert.FromBase64String(RawToken[..RawToken.IndexOf('.')]), out ulong id, out _))
                return id;
            throw new("Invalid token provided.");
        }
    }
}
