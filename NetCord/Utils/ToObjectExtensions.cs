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

    internal static async ValueTask<T> ToObjectAsync<T>(this Stream stream, JsonTypeInfo<T> jsonTypeInfo)
    {
        var result = (await JsonSerializer.DeserializeAsync(stream, jsonTypeInfo).ConfigureAwait(false))!;
        await stream.DisposeAsync().ConfigureAwait(false);
        return result;
    }
}
