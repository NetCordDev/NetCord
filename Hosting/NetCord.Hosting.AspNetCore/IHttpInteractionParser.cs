using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

public interface IHttpInteractionParser
{
    /// <summary>
    /// Parses an incoming HTTP interaction request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> of the incoming request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing the parsed interaction, if validation was successful; otherwise, <see langword="null"/>.</returns>
    public ValueTask<IInteraction?> ParseAsync(HttpContext context);
}

internal sealed class HttpInteractionParser(RestClient client, IOptions<IDiscordOptions> options) : HttpEventParser<IInteraction>(options), IHttpInteractionParser
{
    protected override IInteraction GetData(HttpContext context, ReadOnlySpan<byte> body)
    {
        return HttpInteractionFactory.Create(body, async (interaction, interactionCallback, withResponse, properties, cancellationToken) =>
        {
            using var content = interactionCallback.Serialize();
            context.Response.ContentType = content.Headers.ContentType!.ToString();
            await content.CopyToAsync(context.Response.Body, cancellationToken).ConfigureAwait(false);
            await context.Response.CompleteAsync().ConfigureAwait(false);
            return null;
        }, client);
    }
}
