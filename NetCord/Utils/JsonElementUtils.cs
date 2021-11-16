using System.Buffers;
using System.Text.Json;

namespace NetCord
{
    internal static class JsonElementUtils
    {
        internal static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
                element.WriteTo(writer);
            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }

        internal static T ToObject<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        internal static T ToObject<T>(this JsonDocument document, JsonSerializerOptions options = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            return document.RootElement.ToObject<T>(options);
        }

        internal static object ToObject(this JsonElement element, Type type, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
                element.WriteTo(writer);
            return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, type, options);
        }

        internal static object ToObject(this ref Utf8JsonReader reader, Type type, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize(ref reader, type, options);
        }

        internal static object ToObject(this JsonDocument document, Type type, JsonSerializerOptions options = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            return document.RootElement.ToObject(type, options);
        }
    }
}
