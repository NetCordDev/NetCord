using System.Text.Json.Serialization;

namespace NetCord.Utils;

internal static partial class UInt64Utils
{
    [JsonSerializable(typeof(ulong))]
    public partial class UInt64SerializerContext : JsonSerializerContext
    {
        public static UInt64SerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
