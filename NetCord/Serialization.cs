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

    private static readonly JsonSerializerOptions _options;

    static Serialization()
    {
        _options = new();
        _options.Converters.Add(new JsonConverters.UInt64Converter());
        _options.Converters.Add(new JsonConverters.CultureInfoConverter());
        _options.Converters.Add(new JsonConverters.PermissionConverter());
    }
}
