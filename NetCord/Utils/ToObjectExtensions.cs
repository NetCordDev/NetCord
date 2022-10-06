using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NetCord;

internal static class ToObjectExtensions
{
    internal static readonly JsonSerializerOptions _options;

    static ToObjectExtensions()
    {
        _options = new();
        _options.Converters.Add(new JsonConverters.CultureInfoConverter());
        _options.Converters.Add(new JsonConverters.PermissionConverter());
    }

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

    [RequiresUnreferencedCode("")]
    internal static T ToObject<T>(this JsonElement element)
    {
        return JsonSerializer.Deserialize<T>(element, _options)!;
    }

    [RequiresUnreferencedCode("")]
    internal static T ToObject<T>(this ref Utf8JsonReader reader)
    {
        return JsonSerializer.Deserialize<T>(ref reader, _options)!;
    }

    [RequiresUnreferencedCode("")]
    internal static T ToObject<T>(this JsonDocument document)
    {
        return JsonSerializer.Deserialize<T>(document, _options)!;
    }

    [RequiresUnreferencedCode("")]
    internal static T ToObject<T>(this Stream stream)
    {
        return JsonSerializer.Deserialize<T>(stream, _options)!;
    }
}
