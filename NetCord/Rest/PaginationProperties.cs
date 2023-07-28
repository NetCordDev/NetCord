namespace NetCord.Rest;

public partial record PaginationProperties<T> where T : struct
{
    /// <summary>
    /// The starting point for pagination.
    /// </summary>
    public T? From { get; set; }

    /// <summary>
    /// The direction of pagination.
    /// </summary>
    public PaginationDirection? Direction { get; set; }

    /// <summary>
    /// The number of items to download at once.
    /// </summary>
    public int? Limit { get; set; }

    internal static PaginationProperties<T> Prepare(PaginationProperties<T>? paginationProperties, T minValue, T maxValue, PaginationDirection defaultDirection, int defaultLimit)
    {
        if (paginationProperties is null)
            return new() { Direction = defaultDirection, Limit = defaultLimit };

        PaginationProperties<T> result = new(paginationProperties);

        var direction = result.Direction;
        if (direction.HasValue)
        {
            if (!result.From.HasValue)
            {
                var directionValue = direction.GetValueOrDefault();
                if (directionValue != defaultDirection)
                    result.From = directionValue switch
                    {
                        PaginationDirection.Before => maxValue,
                        PaginationDirection.After => minValue,
                        _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
                    };
            }
        }
        else
            result.Direction = defaultDirection;

        if (!result.Limit.HasValue)
            result.Limit = defaultLimit;

        return result;
    }

    internal static PaginationProperties<T> PrepareWithDirectionValidation(PaginationProperties<T>? paginationProperties, PaginationDirection requiredDirection, int defaultLimit)
    {
        if (paginationProperties is null)
            return new() { Direction = requiredDirection, Limit = defaultLimit };

        PaginationProperties<T> result = new(paginationProperties);

        var direction = result.Direction;
        if (direction.HasValue)
        {
            var directionValue = direction.GetValueOrDefault();
            if (directionValue != requiredDirection)
                throw new ArgumentException($"'{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is required to be '{requiredDirection}' for this endpoint.", nameof(paginationProperties));
        }
        else
            result.Direction = requiredDirection;

        if (!result.Limit.HasValue)
            result.Limit = defaultLimit;

        return result;
    }
}
