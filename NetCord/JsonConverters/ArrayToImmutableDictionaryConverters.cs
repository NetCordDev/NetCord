using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Gateway.JsonModels;
using NetCord.JsonModels;

namespace NetCord.JsonConverters;

internal partial class JsonRoleArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonRole>>
{
    public override ImmutableDictionary<ulong, JsonRole>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonRole.JsonRoleArraySerializerContext.WithOptions.JsonRoleArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonRole> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IEnumerableOfJsonRoleSerializerContext.WithOptions.IEnumerableJsonRole);
    }

    [JsonSerializable(typeof(IEnumerable<JsonRole>))]
    public partial class IEnumerableOfJsonRoleSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonRoleSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

internal partial class JsonChannelArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonChannel>>
{
    public override ImmutableDictionary<ulong, JsonChannel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonChannel.JsonChannelArraySerializerContext.WithOptions.JsonChannelArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonChannel> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IEnumerableOfJsonChannelSerializerContext.WithOptions.IEnumerableJsonChannel);
    }

    [JsonSerializable(typeof(IEnumerable<JsonChannel>))]
    public partial class IEnumerableOfJsonChannelSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonChannelSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

internal partial class JsonStageInstanceArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonStageInstance>>
{
    public override ImmutableDictionary<ulong, JsonStageInstance>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonStageInstanceArraySerializerContext.WithOptions.JsonStageInstanceArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonStageInstance> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IEnumerableOfJsonStageInstanceSerializerContext.WithOptions.IEnumerableJsonStageInstance);
    }

    [JsonSerializable(typeof(JsonStageInstance[]))]
    public partial class JsonStageInstanceArraySerializerContext : JsonSerializerContext
    {
        public static JsonStageInstanceArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<JsonStageInstance>))]
    public partial class IEnumerableOfJsonStageInstanceSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonStageInstanceSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

internal partial class JsonGuildScheduledEventArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonGuildScheduledEvent>>
{
    public override ImmutableDictionary<ulong, JsonGuildScheduledEvent>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildScheduledEvent.JsonGuildScheduledEventArraySerializerContext.WithOptions.JsonGuildScheduledEventArray).ToImmutableDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonGuildScheduledEvent> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IEnumerableOfJsonGuildScheduledEventSerializerContext.WithOptions.IEnumerableJsonGuildScheduledEvent);
    }

    [JsonSerializable(typeof(IEnumerable<JsonGuildScheduledEvent>))]
    public partial class IEnumerableOfJsonGuildScheduledEventSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonGuildScheduledEventSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

internal partial class JsonVoiceStateArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonVoiceState>>
{
    public override ImmutableDictionary<ulong, JsonVoiceState>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonVoiceStateArraySerializerContext.WithOptions.JsonVoiceStateArray).ToImmutableDictionary(v => v.UserId);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonVoiceState> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IEnumerableOfJsonVoiceStateSerializerContext.WithOptions.IEnumerableJsonVoiceState);
    }

    [JsonSerializable(typeof(JsonVoiceState[]))]
    public partial class JsonVoiceStateArraySerializerContext : JsonSerializerContext
    {
        public static JsonVoiceStateArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<JsonVoiceState>))]
    public partial class IEnumerableOfJsonVoiceStateSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonVoiceStateSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

internal partial class JsonGuildUserArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonGuildUser>>
{
    public override ImmutableDictionary<ulong, JsonGuildUser>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).DistinctBy(u => u.User.Id).ToImmutableDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonGuildUser> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IEnumerableOfJsonGuildUserSerializerContext.WithOptions.IEnumerableJsonGuildUser);
    }

    [JsonSerializable(typeof(IEnumerable<JsonGuildUser>))]
    public partial class IEnumerableOfJsonGuildUserSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonGuildUserSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

internal partial class JsonPresenceArrayToImmutableDictionaryConverter : JsonConverter<ImmutableDictionary<ulong, JsonPresence>>
{
    public override ImmutableDictionary<ulong, JsonPresence>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonPresenceArraySerializerContext.WithOptions.JsonPresenceArray).ToImmutableDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, ImmutableDictionary<ulong, JsonPresence> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IEnumerableOfJsonPresenceSerializerContext.WithOptions.IEnumerableJsonPresence);
    }

    [JsonSerializable(typeof(JsonPresence[]))]
    public partial class JsonPresenceArraySerializerContext : JsonSerializerContext
    {
        public static JsonPresenceArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<JsonPresence>))]
    public partial class IEnumerableOfJsonPresenceSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonPresenceSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
