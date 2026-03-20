using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

internal abstract class HttpEventHandler<TRawData> where TRawData : class
{
    public HttpEventHandler(IServiceProvider services, string pattern)
    {
        _services = services;

        var publicKey = services.GetRequiredService<IOptions<IDiscordOptions>>().Value.PublicKey ?? throw new InvalidOperationException($"'{nameof(IDiscordOptions.PublicKey)}' must be set.");
        _validator = new(publicKey);

        _client = services.GetRequiredService<RestClient>();

        Pattern = RoutePatternHelper.ParseLiteral(pattern);
    }

    private protected readonly IServiceProvider _services;

    private protected readonly HttpEventValidator _validator;

    private protected readonly RestClient _client;

    public RoutePattern Pattern { get; }

    protected abstract TRawData GetData(HttpContext context, ReadOnlySpan<byte> body);

    protected abstract ValueTask HandleAsync(HttpContext context, TRawData data);

    protected abstract void LogHandlerException(Exception ex);

    public async Task HandleRequestAsync(HttpContext context)
    {
        var value = await ValidateAsync(context).ConfigureAwait(false);
        if (value is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await HandleAsync(context, value).ConfigureAwait(false);
    }

    private async ValueTask<TRawData?> ValidateAsync(HttpContext context)
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

        if (!_validator.Validate(signatures[0], timestampAndBody.Span))
        {
            ArrayPool<byte>.Shared.Return(timestampAndBodyArray);
            return null;
        }

        var value = GetData(context, timestampAndBody.Span[timestampByteCount..]);

        ArrayPool<byte>.Shared.Return(timestampAndBodyArray);

        return value;
    }

    protected ValueTask InvokeHandlersAsync<THandlerData>(Func<THandlerData, ValueTask>[] handlers, Func<THandlerData> dataFunc) where THandlerData : class
    {
        int length = handlers.Length;

        if (length is 0)
            return default;

        var tasks = ArrayPool<ValueTask>.Shared.Rent(length);

        var data = dataFunc();

        for (int i = 0; i < length; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogHandlerException(ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(length, tasks);
    }

    protected ValueTask InvokeHandlersAsync<THandlerData>(Func<THandlerData, ValueTask>[] handlers, THandlerData data)
    {
        int length = handlers.Length;

        if (length is 0)
            return default;

        var tasks = ArrayPool<ValueTask>.Shared.Rent(length);

        for (int i = 0; i < length; i++)
        {
            try
            {
#pragma warning disable CA2012 // Use ValueTasks correctly
                tasks[i] = handlers[i](data);
#pragma warning restore CA2012 // Use ValueTasks correctly
            }
            catch (Exception ex)
            {
                LogHandlerException(ex);

                tasks[i] = default;
            }
        }

        return HandleTasksAsync(length, tasks);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask HandleTasksAsync(int length, ValueTask[] tasks)
    {
        for (int i = 0; i < length; i++)
        {
            try
            {
                await tasks[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogHandlerException(ex);
            }
        }
    }
}
