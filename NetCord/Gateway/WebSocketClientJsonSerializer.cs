using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Gateway;

internal static class WebSocketClientJsonSerializer
{
    private readonly record struct State(Utf8JsonWriter Writer, RentedArrayBufferWriter<byte> Output);

    private const int DefaultBufferSize = 1024;

    [ThreadStatic]
    private static State? t_state;

    public static RentedArrayBufferWriter<byte>.RentedBufferOwner Serialize<T>(T value, JsonTypeInfo<T> jsonTypeInfo)
    {
        if (t_state is (var writer, var output))
        {
            writer.Reset();
            output.Clear();
        }
        else
        {
            output = new(DefaultBufferSize);
            writer = new(output);
            t_state = new(writer, output);
        }

        JsonSerializer.Serialize(writer, value, jsonTypeInfo);

        return output.ExtractBuffer();
    }
}
