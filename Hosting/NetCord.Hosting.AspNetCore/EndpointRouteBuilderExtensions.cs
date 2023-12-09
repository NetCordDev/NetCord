using System.Buffers;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseHttpInteractions(this IEndpointRouteBuilder endpoints, string pattern)
    {
        var publicKey = endpoints.ServiceProvider.GetRequiredService<IOptions<IDiscordOptions>>().Value.PublicKey;
        HttpInteractionValidator validator = new(publicKey);

        var handlers = endpoints.ServiceProvider.GetServices<IHttpInteractionHandler>().ToArray();
        var tasks = new ValueTask[handlers.Length];

        var client = endpoints.ServiceProvider.GetRequiredService<RestClient>();

        var routePattern = RoutePatternHelper.ParseLiteral(pattern);

        endpoints
            .Map(routePattern, context => HandleRequestAsync(context, validator, handlers, tasks, client))
            .WithMetadata(new HttpMethodMetadata([HttpMethods.Post]));

        return endpoints;
    }

    private static async Task HandleRequestAsync(HttpContext context, HttpInteractionValidator validator, IHttpInteractionHandler[] handlers, ValueTask[] tasks, RestClient client)
    {
        var iInteraction = await ParseInteractionAsync(context, validator, client).ConfigureAwait(false);

        switch (iInteraction)
        {
            case Interaction interaction:
                await HandleInteractionAsync(interaction, handlers, tasks).ConfigureAwait(false);
                break;
            case PingInteraction pingInteraction:
                await pingInteraction.SendResponseAsync(InteractionCallback.Pong).ConfigureAwait(false);
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                break;
        }
    }

    private static async ValueTask<IInteraction?> ParseInteractionAsync(HttpContext context, HttpInteractionValidator validator, RestClient client)
    {
        var request = context.Request;

        var headers = request.Headers;
        if (!headers.TryGetValue("X-Signature-Ed25519", out var signatures) || !headers.TryGetValue("X-Signature-Timestamp", out var timestamps))
            return null;

        var timestamp = timestamps[0]!;
        int timestampByteCount = Encoding.UTF8.GetByteCount(timestamp);

        int timestampAndBodyLength = timestampByteCount + (int)request.ContentLength.GetValueOrDefault();

        var timestampAndBodyArray = ArrayPool<byte>.Shared.Rent(timestampAndBodyLength);
        var timestampAndBody = timestampAndBodyArray.AsMemory(0, timestampAndBodyLength);

        Encoding.UTF8.GetBytes(timestamp, timestampAndBody.Span);

        int position = timestampByteCount;
        var body = request.Body;
        while (position < timestampAndBodyLength)
            position += await body.ReadAsync(timestampAndBody[position..]).ConfigureAwait(false);

        if (!validator.Validate(signatures[0], timestampAndBody.Span))
            return null;

        var response = context.Response;
        var interaction = HttpInteractionFactory.Create(timestampAndBody.Span[timestampByteCount..], async (interaction, interactionCallback, properties) =>
        {
            using var content = interactionCallback.Serialize();
            response.ContentType = content.Headers.ContentType!.ToString();
            await content.CopyToAsync(response.Body).ConfigureAwait(false);
            await response.CompleteAsync().ConfigureAwait(false);
        }, client);

        ArrayPool<byte>.Shared.Return(timestampAndBodyArray);

        return interaction;
    }

    private static async ValueTask HandleInteractionAsync(Interaction interaction, IHttpInteractionHandler[] handlers, ValueTask[] tasks)
    {
        int length = handlers.Length;

        for (int i = 0; i < length; i++)
            tasks[i] = handlers[i].HandleAsync(interaction);

        for (int i = 0; i < length; i++)
            await tasks[i].ConfigureAwait(false);
    }
}
