using System.Buffers;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using NetCord.Rest;
using Microsoft.Extensions.DependencyInjection;
namespace NetCord.Hosting.AzureFunctions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="DiscordInteractionsHandler"/> to the service collection.
    /// </summary>
    public static IServiceCollection AddDiscordInteractions(this IServiceCollection services)
    {
        services.AddSingleton<DiscordInteractionsHandler>();
        return services;
    }
}

public class DiscordInteractionsHandler
{
    private readonly IHttpInteractionHandler[] _handlers;
    private readonly ValueTask[] _tasks;
    private readonly HttpInteractionValidator _validator;
    private readonly RestClient _client;

    public DiscordInteractionsHandler(
        IOptions<IDiscordOptions> options,
        IEnumerable<IHttpInteractionHandler> handlers,
        RestClient client)
    {
        var publicKey = options.Value.PublicKey ?? throw new InvalidOperationException("PublicKey must be set in IDiscordOptions.");
        _validator = new HttpInteractionValidator(publicKey);
        _handlers = handlers.ToArray();
        _tasks = new ValueTask[_handlers.Length];
        _client = client;
    }

    /// <summary>
    /// Handles the Discord interaction request.
    /// </summary>
    /// <param name="req">The HTTP request data.</param>
    /// <param name="context">The function context.</param>
    /// <returns>The result of the interaction.</returns>
    public async Task<HttpResponseData> HandleRequestAsync(HttpRequestData req, FunctionContext context)
    {
        var httpContext = context.GetHttpContext();
        if (httpContext == null)
        {
            var res = req.CreateResponse(HttpStatusCode.InternalServerError);
            await res.WriteStringAsync("HttpContext not available.").ConfigureAwait(false);
            return res;
        }

        var interaction = await ParseInteractionAsync(httpContext, _validator, _client).ConfigureAwait(false);
        if (interaction == null)
        {
            var res = req.CreateResponse(HttpStatusCode.Unauthorized);
            return res;
        }

        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

        switch (interaction)
        {
            case Interaction realInteraction:
                await HandleInteractionAsync(realInteraction, _handlers, _tasks).ConfigureAwait(false);
                break;

            case PingInteraction ping:
                await ping.SendResponseAsync(InteractionCallback.Pong).ConfigureAwait(false);
                break;

            default:
                response.StatusCode = HttpStatusCode.Unauthorized;
                break;
        }

        return response;
    }

    private static async ValueTask<IInteraction?> ParseInteractionAsync(HttpContext context, HttpInteractionValidator validator, RestClient client)
    {
        var request = context.Request;

        var headers = request.Headers;
        if (!headers.TryGetValue("X-Signature-Ed25519", out var signatures) || !headers.TryGetValue("X-Signature-Timestamp", out var timestamps))
        {
            return null;
        }

        var timestamp = timestamps[0]!;
        int timestampByteCount = Encoding.UTF8.GetByteCount(timestamp);

        int timestampAndBodyLength = timestampByteCount + (int)request.ContentLength.GetValueOrDefault();

        var timestampAndBodyArray = ArrayPool<byte>.Shared.Rent(timestampAndBodyLength);
        var timestampAndBody = timestampAndBodyArray.AsMemory(0, timestampAndBodyLength);

        Encoding.UTF8.GetBytes(timestamp, timestampAndBody.Span);

        int position = timestampByteCount;
        var body = request.Body;
        while (position < timestampAndBodyLength)
        {
            position += await body.ReadAsync(timestampAndBody[position..]).ConfigureAwait(false);
        }

        if (!validator.Validate(signatures[0], timestampAndBody.Span))
        {
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

    private static async ValueTask HandleInteractionAsync(Interaction interaction, IHttpInteractionHandler[] handlers, ValueTask[] tasks)
    {
        int length = handlers.Length;

        for (int i = 0; i < length; i++)
#pragma warning disable CA2012
        {
            tasks[i] = handlers[i].HandleAsync(interaction);
        }
#pragma warning restore CA2012

        for (int i = 0; i < length; i++)
        {
            await tasks[i].ConfigureAwait(false);
        }
    }
}

