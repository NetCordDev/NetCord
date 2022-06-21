using System.Text.Json;

namespace NetCord;

public static class Serialization
{
    public static JsonSerializerOptions Options => new(ToObjectExtensions._options);
}