namespace NetCord.Rest;

internal sealed class PaginationQueryBuilder<T>(int batchSize,
                                                string fromQueryName,
                                                Func<T, string> toString,
                                                string baseQuery = "?") where T : struct
{
    public PaginationQueryBuilder(int batchSize,
                                  PaginationDirection direction,
                                  Func<T, string> toString,
                                  string baseQuery = "?") : this(batchSize,
                                                                 GetFromQueryName(direction),
                                                                 toString,
                                                                 baseQuery)
    {
    }

    private static string GetFromQueryName(PaginationDirection direction)
    {
        return direction switch
        {
            PaginationDirection.Before => "before",
            PaginationDirection.After => "after",
            _ => throw new ArgumentException($"The value of '{nameof(direction)}' is invalid."),
        };
    }

    public override string ToString() => $"{baseQuery}limit={batchSize}";

    public string ToString(T? from)
    {
        if (from.HasValue)
            return $"{baseQuery}limit={batchSize}&{fromQueryName}={toString(from.GetValueOrDefault())}";

        return ToString();
    }
}
