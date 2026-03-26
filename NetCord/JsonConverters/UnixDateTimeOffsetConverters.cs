using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

// We need to narrow the range of Unix timestamps as Discord can send values that are outside the valid range for 'DateTimeOffset'

public class MillisecondsUnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.FromUnixTimeMilliseconds(Helper.NarrowMilliseconds(reader.GetInt64()));

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}

public class SecondsUnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.FromUnixTimeSeconds(Helper.NarrowSeconds(reader.GetInt64()));

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}

public class MillisecondsNullableUnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.FromUnixTimeMilliseconds(Helper.NarrowMilliseconds(reader.GetInt64()));

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.GetValueOrDefault().ToUnixTimeMilliseconds());
    }
}

public class SecondsNullableUnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.FromUnixTimeSeconds(Helper.NarrowSeconds(reader.GetInt64()));

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.GetValueOrDefault().ToUnixTimeSeconds());
    }
}

file class Helper
{
    public static long NarrowMilliseconds(long milliseconds)
    {
        return milliseconds switch
        {
            <= -62135596800000 => -62135596800000,
            >= 253402300799999 => 253402300799999,
            _ => milliseconds
        };
    }

    public static long NarrowSeconds(long seconds)
    {
        return seconds switch
        {
            <= -62135596800 => -62135596800,
            >= 253402300799 => 253402300799,
            _ => seconds
        };
    }
}
