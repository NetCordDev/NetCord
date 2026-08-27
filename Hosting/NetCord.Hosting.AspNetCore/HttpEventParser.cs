using System.Buffers;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.AspNetCore;

internal abstract class HttpEventParser<TRawData>
{
    private readonly HttpEventValidator _validator;

    public HttpEventParser(IOptions<IDiscordOptions> options)
    {
        var publicKey = options.Value.PublicKey ?? throw new InvalidOperationException($"'{nameof(IDiscordOptions.PublicKey)}' must be set.");

        _validator = new(publicKey);
    }

    public async ValueTask<TRawData?> ParseAsync(HttpContext context)
    {
        var request = context.Request;

        var headers = request.Headers;
        if (!headers.TryGetValue("X-Signature-Ed25519", out var signatures) || !headers.TryGetValue("X-Signature-Timestamp", out var timestamps))
            return default;

        var timestamp = timestamps[0]!;
        int timestampByteCount = Encoding.UTF8.GetByteCount(timestamp);

        int timestampAndBodyLength = timestampByteCount + (int)request.ContentLength.GetValueOrDefault();

        var timestampAndBodyArray = ArrayPool<byte>.Shared.Rent(timestampAndBodyLength);

        try
        {
            var timestampAndBody = timestampAndBodyArray.AsMemory(0, timestampAndBodyLength);

            Encoding.UTF8.GetBytes(timestamp, timestampAndBody.Span);

            await request.Body.ReadExactlyAsync(timestampAndBody[timestampByteCount..]).ConfigureAwait(false);

            return _validator.Validate(signatures[0], timestampAndBody.Span)
                ? GetData(context, timestampAndBody.Span[timestampByteCount..])
                : default;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(timestampAndBodyArray);
        }
    }

    protected abstract TRawData GetData(HttpContext context, ReadOnlySpan<byte> body);
}

