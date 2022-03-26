using System.Text.Json;

namespace NetCord;

internal static class ToObjectExtensions
{
    internal static readonly JsonSerializerOptions _options;

    static ToObjectExtensions()
    {
        _options = new();
        _options.Converters.Add(new JsonConverters.CultureInfoConverter());
    }

    internal static T ToObject<T>(this JsonElement element)
    {
        return JsonSerializer.Deserialize<T>(element, _options)!;
    }

    internal static T ToObject<T>(this ref Utf8JsonReader reader)
    {
        return JsonSerializer.Deserialize<T>(ref reader, _options)!;
    }

    internal static T ToObject<T>(this JsonDocument document)
    {
        return JsonSerializer.Deserialize<T>(document, _options)!;
    }

    internal static T ToObject<T>(this Stream stream)
    {
        return JsonSerializer.Deserialize<T>(stream, _options)!;
    }
}