using System.Text.Json.Serialization;

namespace NetCord;

public enum MfaLevel
{
    None = 0,
    Elevated = 1,
}

[JsonSerializable(typeof(MfaLevel))]
internal partial class MfaLevelSerializerContext : JsonSerializerContext
{
    public static MfaLevelSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
}
