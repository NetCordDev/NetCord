using System.Text.Json;

namespace NetCord;

internal static class JsonElementUtils
{
    internal static T ToObject<T>(this JsonElement element, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(element, options)!;
    }

    internal static T ToObject<T>(this ref Utf8JsonReader reader, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(ref reader, options)!;
    }

    internal static T ToObject<T>(this JsonDocument document, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(document, options)!;
    }

    internal static object ToObject(this JsonElement element, Type type, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(element, type, options)!;
    }

    internal static object ToObject(this ref Utf8JsonReader reader, Type type, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(ref reader, type, options)!;
    }

    internal static object ToObject(this JsonDocument document, Type type, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(document, type, options)!;
    }
}