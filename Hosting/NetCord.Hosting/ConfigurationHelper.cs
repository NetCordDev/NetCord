using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting;

internal static class ConfigurationHelper
{
    public static IToken ParseToken(string token, IServiceProvider services)
    {
        var factory = services.GetService<ITokenFactory>();
        if (factory is not null)
            return factory.CreateToken(token);

        var spaceIndex = token.IndexOf(' ');
        if (spaceIndex < 0)
            return new BotToken(token);

        return token.AsSpan(0, spaceIndex) switch
        {
            "Bot" => new BotToken(token[(spaceIndex + 1)..]),
            "Bearer" => new BearerToken(token[(spaceIndex + 1)..]),
            _ => throw new ArgumentException("Unknown token type.", nameof(token)),
        };
    }
}
