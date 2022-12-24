using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NetCord;

internal static class ToObjectExtensions
{
    internal static T ToObject<T>(this JsonElement element, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize(element, jsonTypeInfo)!;
    }

    internal static T ToObject<T>(this ref Utf8JsonReader reader, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize(ref reader, jsonTypeInfo)!;
    }

    internal static T ToObject<T>(this JsonDocument document, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize(document, jsonTypeInfo)!;
    }

    internal static T ToObject<T>(this Stream stream, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize(stream, jsonTypeInfo)!;
    }
}
