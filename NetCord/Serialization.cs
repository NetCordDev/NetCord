using System.Runtime.CompilerServices;
using System.Text.Json;

namespace NetCord;

public static class Serialization
{
    public static JsonSerializerOptions Options
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_options);
    }

    internal static JsonSerializerOptions OriginalOptions => _options;

    private static readonly JsonSerializerOptions _options;

    static Serialization()
    {
        JsonSerializerOptions options = new();
        options.Converters.Add(new JsonConverters.UInt64Converter());
        options.Converters.Add(new JsonConverters.CultureInfoConverter());
        options.Converters.Add(new JsonConverters.PermissionsConverter());
        _options = options;
    }
}
