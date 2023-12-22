using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NetCord;

internal static class SerializationUtils
{
    internal static T ToObject<T>(this JsonElement element, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize(element, jsonTypeInfo)!;
    }

    internal static T ToObject<T>(this ref Utf8JsonReader reader, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize(ref reader, jsonTypeInfo)!;
    }

    internal static async ValueTask<T> ToObjectAsync<T>(this Stream stream, JsonTypeInfo<T> jsonTypeInfo)
    {
        try
        {
            return (await JsonSerializer.DeserializeAsync(stream, jsonTypeInfo).ConfigureAwait(false))!;
        }
        finally
        {
            await stream.DisposeAsync().ConfigureAwait(false);
        }
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(JsonElement, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(JsonElement, JsonSerializerOptions)")]
    internal static T ToObject<T>(this JsonElement element)
    {
        return JsonSerializer.Deserialize<T>(element, Serialization.Default.Options)!;
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(ref Utf8JsonReader, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(ref Utf8JsonReader, JsonSerializerOptions)")]
    internal static T ToObject<T>(this ref Utf8JsonReader reader)
    {
        return JsonSerializer.Deserialize<T>(ref reader, Serialization.Default.Options)!;
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    internal static async ValueTask<T> ToObjectAsync<T>(this Stream stream)
    {
        try
        {
            return (await JsonSerializer.DeserializeAsync<T>(stream, Serialization.Default.Options).ConfigureAwait(false))!;
        }
        finally
        {
            await stream.DisposeAsync().ConfigureAwait(false);
        }
    }
}
