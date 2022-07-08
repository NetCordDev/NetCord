using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.JsonConverters;

internal class ArrayToDictionaryConverter<TValue> : JsonConverter<Dictionary<Snowflake, TValue>> where TValue : JsonModels.JsonEntity
{
    public override Dictionary<Snowflake, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject<TValue[]>().ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, TValue> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, options);
    }
}

internal class JsonVoiceStateArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonVoiceState>>
{
    public override Dictionary<Snowflake, JsonVoiceState>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject<JsonVoiceState[]>().ToDictionary(v => v.UserId);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonVoiceState> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, options);
    }
}

internal class JsonGuildUserArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonGuildUser>>
{
    public override Dictionary<Snowflake, JsonGuildUser>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject<JsonGuildUser[]>().DistinctBy(u => u.User.Id).ToDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonGuildUser> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, options);
    }
}

internal class JsonPresenceArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonPresence>>
{
    public override Dictionary<Snowflake, JsonPresence>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject<JsonPresence[]>().ToDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonPresence> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, options);
    }
}