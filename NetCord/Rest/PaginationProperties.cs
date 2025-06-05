namespace NetCord.Rest;

public partial record PaginationProperties<T> : IPaginationProperties<T, PaginationProperties<T>> where T : struct
{
    public T? From { get; set; }

    public PaginationDirection? Direction { get; set; }

    public int? BatchSize { get; set; }

    internal static TProperties Prepare<TProperties>(TProperties? paginationProperties, T minValue, T maxValue, PaginationDirection defaultDirection, int defaultLimit) where TProperties : IPaginationProperties<T, TProperties>
    {
        if (paginationProperties is null)
        {
            var properties = TProperties.Create();
            properties.Direction = defaultDirection;
            properties.BatchSize = defaultLimit;
            return properties;
        }

        var result = TProperties.Create(paginationProperties);

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

        if (!result.BatchSize.HasValue)
            result.BatchSize = defaultLimit;

        return result;
    }

    internal static TProperties PrepareWithDirectionValidation<TProperties>(TProperties? paginationProperties, PaginationDirection requiredDirection, int defaultLimit) where TProperties : IPaginationProperties<T, TProperties>
    {
        if (paginationProperties is null)
        {
            var properties = TProperties.Create();
            properties.Direction = requiredDirection;
            properties.BatchSize = defaultLimit;
            return properties;
        }

        var result = TProperties.Create(paginationProperties);

        var direction = result.Direction;
        if (direction.HasValue)
        {
            var directionValue = direction.GetValueOrDefault();
            if (directionValue != requiredDirection)
                throw new ArgumentException($"'{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is required to be '{requiredDirection}' for this endpoint.", nameof(paginationProperties));
        }
        else
            result.Direction = requiredDirection;

        if (!result.BatchSize.HasValue)
            result.BatchSize = defaultLimit;

        return result;
    }

    static PaginationProperties<T> IPaginationProperties<T, PaginationProperties<T>>.Create() => new();
    static PaginationProperties<T> IPaginationProperties<T, PaginationProperties<T>>.Create(PaginationProperties<T> properties) => new(properties);
}
