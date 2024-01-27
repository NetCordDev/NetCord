using System.Text;

namespace NetCord.Rest;

internal class PaginationQueryBuilder<T>(int limit, PaginationDirection direction, Func<T, string> toString, string baseQuery = "?") where T : struct
{
    private readonly string _direction = direction switch
    {
        PaginationDirection.Before => "before",
        PaginationDirection.After => "after",
        _ => throw new ArgumentException($"The value of '{nameof(direction)}' is invalid."),
    };

    private readonly StringBuilder _builder = new StringBuilder(baseQuery).Append("limit=").Append(limit);

    public override string ToString() => _builder.ToString();

    public string ToString(T? from)
    {
        var builder = _builder;

        if (from.HasValue)
        {
            int length = builder.Length;

            builder.Append('&')
                   .Append(_direction)
                   .Append('=')
                   .Append(toString(from.GetValueOrDefault()));

            var result = builder.ToString();

            builder.Length = length;

            return result;
        }

        return builder.ToString();
    }
}
