using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Gateway.JsonModels;
using NetCord.JsonModels;

namespace NetCord.JsonConverters;

public partial class JsonRoleArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonRole>>
{
    public override ImmutableDictionary<ulong, JsonRole>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonRoleArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonRole> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, Serialization.Default.IEnumerableJsonRole);
    }
}

public partial class JsonChannelArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonChannel>>
{
    public override ImmutableDictionary<ulong, JsonChannel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonChannelArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonChannel> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, Serialization.Default.IEnumerableJsonChannel);
    }
}

public partial class JsonStageInstanceArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonStageInstance>>
{
    public override ImmutableDictionary<ulong, JsonStageInstance>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonStageInstanceArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonStageInstance> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, Serialization.Default.IEnumerableJsonStageInstance);
    }
}

public partial class JsonGuildScheduledEventArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonGuildScheduledEvent>>
{
    public override ImmutableDictionary<ulong, JsonGuildScheduledEvent>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonGuildScheduledEventArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonGuildScheduledEvent> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, Serialization.Default.IEnumerableJsonGuildScheduledEvent);
    }
}

public partial class JsonVoiceStateArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonVoiceState>>
{
    public override ImmutableDictionary<ulong, JsonVoiceState>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonVoiceStateArray).ToImmutableDictionary(v => v.UserId);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonVoiceState> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, Serialization.Default.IEnumerableJsonVoiceState);
    }
}

public partial class JsonGuildUserArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonGuildUser>>
{
    public override ImmutableDictionary<ulong, JsonGuildUser>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonGuildUserArray).DistinctBy(u => u.User.Id).ToImmutableDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonGuildUser> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, Serialization.Default.IEnumerableJsonGuildUser);
    }
}

public partial class JsonPresenceArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonPresence>>
{
    public override ImmutableDictionary<ulong, JsonPresence>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonPresenceArray).ToImmutableDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonPresence> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, Serialization.Default.IEnumerableJsonPresence);
    }
}
