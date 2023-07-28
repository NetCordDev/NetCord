using System.Text;

namespace NetCord.Rest;

internal class PaginationQueryBuilder<T> where T : struct
{
    public PaginationQueryBuilder(int limit, Func<T, string> toString, string baseQuery = "?")
    {
        _builder = new StringBuilder(baseQuery).Append("limit=").Append(limit);
        _toString = toString;
    }

    private readonly StringBuilder _builder;
    private readonly Func<T, string> _toString;

    public override string ToString() => _builder.ToString();

    public string ToString(T? from, PaginationDirection direction)
    {
        var builder = _builder;

        if (from.HasValue)
        {
            var length = builder.Length;
            builder.Append('&')
                   .Append(direction switch
                   {
                       PaginationDirection.Before => "before",
                       PaginationDirection.After => "after",
                       _ => throw new ArgumentException($"The value of '{nameof(direction)}' is invalid."),
                   })
                   .Append('=')
                   .Append(_toString(from.GetValueOrDefault()));

            var result = builder.ToString();

            builder.Length = length;

            return result;
        }

        return builder.ToString();
    }
}
