using System.Buffers;
using System.Text;

namespace NetCord.Rest.RateLimits;

public record Route : ISpanFormattable
{
    public Route(HttpMethod method, string endPoint, TopLevelResourceInfo? resourceInfo = null)
    {
        Method = method;
        EndPoint = endPoint;
        ResourceInfo = resourceInfo;
    }

    public HttpMethod Method { get; }

    public string EndPoint { get; }

    public TopLevelResourceInfo? ResourceInfo { get; }

    public override string ToString()
    {
        return $"{Method.Method} {GetFormattedEndPoint()}";
    }

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var method = Method.Method;
        int methodLength = method.Length;

        int destinationLength = destination.Length;

        if (destinationLength <= methodLength)
        {
            charsWritten = 0;
            return false;
        }

        var formattedEndPoint = GetFormattedEndPoint();
        int formattedEndPointLength = formattedEndPoint.Length;

        if (destinationLength <= methodLength + formattedEndPointLength)
        {
            charsWritten = 0;
            return false;
        }

        method.CopyTo(destination);
        destination[methodLength] = ' ';
        formattedEndPoint.CopyTo(destination[(methodLength + 1)..]);

        charsWritten = methodLength + formattedEndPointLength + 1;

        return true;
    }

    private string GetFormattedEndPoint()
    {
        if (ResourceInfo is { } resourceInfo)
        {
            var endPoint = EndPoint;

            var compositeFormat = CompositeFormat.Parse(endPoint);

            int argumentCount = compositeFormat.MinimumArgumentCount;

            if (argumentCount is 0)
                return endPoint;

            var arguments = ArrayPool<object?>.Shared.Rent(argumentCount);

            arguments[0] = resourceInfo.ResourceId;

            if (argumentCount > 1)
            {
                arguments[1] = resourceInfo.ResourceToken ?? "{1}";

                for (int i = 2; i < argumentCount; i++)
                    arguments[i] = $"{{{i}}}";
            }

            var formattedEndPoint = string.Format(null, compositeFormat, arguments.AsSpan(0, argumentCount));

            ArrayPool<object?>.Shared.Return(arguments);

            return formattedEndPoint;
        }

        return EndPoint;
    }
}
