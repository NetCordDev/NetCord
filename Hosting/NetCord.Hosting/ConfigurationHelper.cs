namespace NetCord.Hosting;

internal static class ConfigurationHelper
{
    public static Token ParseToken(string token)
    {
        var spaceIndex = token.IndexOf(' ');
        TokenType tokenType;
        string tokenString;
        if (spaceIndex == -1)
        {
            tokenType = TokenType.Bot;
            tokenString = token;
        }
        else
        {
            tokenType = token.AsSpan(0, spaceIndex) switch
            {
                "Bot" => TokenType.Bot,
                "Bearer" => TokenType.Bearer,
                _ => (TokenType)(-1),
            };
            tokenString = token[(spaceIndex + 1)..];
        }
        return new(tokenType, tokenString);
    }
}
