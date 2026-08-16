using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

public interface IWebhookEventParser
{
    /// <summary>
    /// Parses an incoming webhook event request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the incoming request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing the parsed webhook event data.</returns>
    public ValueTask<IWebhookEventArgs?> ParseAsync(HttpContext context);
}

internal sealed class WebhookEventParser(RestClient client, IOptions<IDiscordOptions> options) : HttpEventParser<IWebhookEventArgs>(options), IWebhookEventParser
{
    protected override IWebhookEventArgs GetData(HttpContext context, ReadOnlySpan<byte> body)
    {
        return IWebhookEventArgs.CreateFromJson(WebhookEventArgsFactory.CreateJson(body), client);
    }
}
