using System.Text.Json;

namespace NetCord;

internal static class ToObjectExtensions
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

    internal static T ToObject<T>(this Stream stream, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(stream, options)!;
    }
}