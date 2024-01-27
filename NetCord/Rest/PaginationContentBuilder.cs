using System.Text.Json.Serialization.Metadata;

namespace NetCord.Rest;

internal class PaginationContentBuilder<T, TFrom>(T paginationProperties, JsonTypeInfo<T> jsonTypeInfo) where T : PaginationProperties<TFrom> where TFrom : struct
{
    public JsonContent<T> Build(TFrom? from)
    {
        paginationProperties.From = from;
        return new(paginationProperties, jsonTypeInfo);
    }
}
