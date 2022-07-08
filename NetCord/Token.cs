namespace NetCord;

public class Token
{
    public Token(TokenType type, string token!!)
    {
        Type = type;
        RawToken = token;
    }

    public TokenType Type { get; }
    public string RawToken { get; }

    public string ToHttpHeader() => Type == TokenType.Bot ? $"Bot {RawToken}" : RawToken;

    public Snowflake Id => new(System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(RawToken[..RawToken.IndexOf('.')])));
}