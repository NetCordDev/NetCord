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

    internal const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";

    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    internal static T ToObject<T>(this JsonElement element)
    {
        return JsonSerializer.Deserialize<T>(element, Serialization.OriginalOptions)!;
    }

    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    internal static T ToObject<T>(this ref Utf8JsonReader reader)
    {
        return JsonSerializer.Deserialize<T>(ref reader, Serialization.OriginalOptions)!;
    }

    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    internal static async ValueTask<T> ToObjectAsync<T>(this Stream stream)
    {
        try
        {
            return (await JsonSerializer.DeserializeAsync<T>(stream, Serialization.OriginalOptions).ConfigureAwait(false))!;
        }
        finally
        {
            await stream.DisposeAsync().ConfigureAwait(false);
        }
    }
}
