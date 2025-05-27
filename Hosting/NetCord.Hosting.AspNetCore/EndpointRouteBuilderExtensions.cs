using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Adds a route to the <see cref="IEndpointRouteBuilder"/> that will handle Discord interactions.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder UseHttpInteractions(this IEndpointRouteBuilder endpoints, string pattern)
    {
        var services = endpoints.ServiceProvider;

        var publicKey = services.GetRequiredService<IOptions<IDiscordOptions>>().Value.PublicKey ?? throw new InvalidOperationException($"'{nameof(IDiscordOptions.PublicKey)}' must be set.");
        HttpInteractionValidator validator = new(publicKey);

        var handlers = services.GetServices<IHttpInteractionHandler>().ToArray();
        var tasks = new ValueTask[handlers.Length];

        var client = services.GetRequiredService<RestClient>();

        var logger = services.GetRequiredService<ILogger<IHttpInteractionHandler>>();

        var routePattern = RoutePatternHelper.ParseLiteral(pattern);

        return endpoints
            .Map(routePattern, context => HandleRequestAsync(context, validator, handlers, tasks, client, logger))
            .WithMetadata(new HttpMethodMetadata([HttpMethods.Post]));
    }

    private static async Task HandleRequestAsync(HttpContext context, HttpInteractionValidator validator, IHttpInteractionHandler[] handlers, ValueTask[] tasks, RestClient client, ILogger<IHttpInteractionHandler> logger)
    {
        var iInteraction = await ParseInteractionAsync(context, validator, client).ConfigureAwait(false);

        switch (iInteraction)
        {
            case Interaction interaction:
                await HandleInteractionAsync(interaction, handlers, tasks, logger).ConfigureAwait(false);
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

        await request.Body.ReadExactlyAsync(timestampAndBody[timestampByteCount..]).ConfigureAwait(false);

        if (!validator.Validate(signatures[0], timestampAndBody.Span))
        {
            ArrayPool<byte>.Shared.Return(timestampAndBodyArray);
            return null;
        }

        var response = context.Response;
        var interaction = HttpInteractionFactory.Create(timestampAndBody.Span[timestampByteCount..], async (interaction, interactionCallback, properties, cancellationToken) =>
        {
            using var content = interactionCallback.Serialize();
            response.ContentType = content.Headers.ContentType!.ToString();
            await content.CopyToAsync(response.Body, cancellationToken).ConfigureAwait(false);
            await response.CompleteAsync().ConfigureAwait(false);
        }, client);

        ArrayPool<byte>.Shared.Return(timestampAndBodyArray);

        return interaction;
    }

    private static ValueTask HandleInteractionAsync(Interaction interaction, IHttpInteractionHandler[] handlers, ValueTask[] tasks, ILogger<IHttpInteractionHandler> logger)
    {
        int length = handlers.Length;

        for (int i = 0; i < length; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i].HandleAsync(interaction);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogHandlerException(logger, ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(length, tasks, logger);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private static async ValueTask HandleTasksAsync(int length, ValueTask[] tasks, ILogger<IHttpInteractionHandler> logger)
    {
        for (int i = 0; i < length; i++)
        {
            try
            {
                await tasks[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogHandlerException(logger, ex);
            }
        }
    }

    private static void LogHandlerException(ILogger<IHttpInteractionHandler> logger, Exception ex)
    {
        logger.LogError(ex, "An error occurred while invoking an HTTP interaction handler.");
    }
}
