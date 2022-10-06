using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.JsonConverters;

//internal class ArrayToDictionaryConverter<TValue> : JsonConverter<Dictionary<Snowflake, TValue>> where TValue : JsonModels.JsonEntity
//{
//    public override Dictionary<Snowflake, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        return reader.ToObject<TValue[]>().ToDictionary(v => v.Id);
//    }

//    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, TValue> value, JsonSerializerOptions options)
//    {
//        JsonSerializer.Serialize(writer, value.Values, options);
//    }
//}

internal partial class JsonGuildRoleArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonGuildRole>>
{
    public override Dictionary<Snowflake, JsonGuildRole>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildRole.JsonGuildRoleArraySerializerContext.WithOptions.JsonGuildRoleArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonGuildRole> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonGuildRoleSerializerContext.WithOptions.IReadOnlyCollectionJsonGuildRole);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonGuildRole>))]
    public partial class IReadOnlyCollectionOfJsonGuildRoleSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonGuildRoleSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonChannelArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonChannel>>
{
    public override Dictionary<Snowflake, JsonChannel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonChannel.JsonChannelArraySerializerContext.WithOptions.JsonChannelArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonChannel> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonChannelSerializerContext.WithOptions.IReadOnlyCollectionJsonChannel);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonChannel>))]
    public partial class IReadOnlyCollectionOfJsonChannelSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonChannelSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonStageInstanceArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonStageInstance>>
{
    public override Dictionary<Snowflake, JsonStageInstance>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonStageInstanceArraySerializerContext.WithOptions.JsonStageInstanceArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonStageInstance> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonStageInstanceSerializerContext.WithOptions.IReadOnlyCollectionJsonStageInstance);
    }

    [JsonSerializable(typeof(JsonStageInstance[]))]
    public partial class JsonStageInstanceArraySerializerContext : JsonSerializerContext
    {
        public static JsonStageInstanceArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonStageInstance>))]
    public partial class IReadOnlyCollectionOfJsonStageInstanceSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonStageInstanceSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonGuildScheduledEventArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonGuildScheduledEvent>>
{
    public override Dictionary<Snowflake, JsonGuildScheduledEvent>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildScheduledEvent.JsonGuildScheduledEventArraySerializerContext.WithOptions.JsonGuildScheduledEventArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonGuildScheduledEvent> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonGuildScheduledEventSerializerContext.WithOptions.IReadOnlyCollectionJsonGuildScheduledEvent);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonGuildScheduledEvent>))]
    public partial class IReadOnlyCollectionOfJsonGuildScheduledEventSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonGuildScheduledEventSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonVoiceStateArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonVoiceState>>
{
    public override Dictionary<Snowflake, JsonVoiceState>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonVoiceStateArraySerializerContext.WithOptions.JsonVoiceStateArray).ToDictionary(v => v.UserId);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonVoiceState> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonVoiceStateSerializerContext.WithOptions.IReadOnlyCollectionJsonVoiceState);
    }

    [JsonSerializable(typeof(JsonVoiceState[]))]
    public partial class JsonVoiceStateArraySerializerContext : JsonSerializerContext
    {
        public static JsonVoiceStateArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonVoiceState>))]
    public partial class IReadOnlyCollectionOfJsonVoiceStateSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonVoiceStateSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonGuildUserArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonGuildUser>>
{
    public override Dictionary<Snowflake, JsonGuildUser>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).DistinctBy(u => u.User.Id).ToDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonGuildUser> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonGuildUserSerializerContext.WithOptions.IReadOnlyCollectionJsonGuildUser);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonGuildUser>))]
    public partial class IReadOnlyCollectionOfJsonGuildUserSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonGuildUserSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonPresenceArrayToDictionaryConverter : JsonConverter<Dictionary<Snowflake, JsonPresence>>
{
    public override Dictionary<Snowflake, JsonPresence>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonPresenceArraySerializerContext.WithOptions.JsonPresenceArray).ToDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Snowflake, JsonPresence> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonPresenceSerializerContext.WithOptions.IReadOnlyCollectionJsonPresence);
    }

    [JsonSerializable(typeof(JsonPresence[]))]
    public partial class JsonPresenceArraySerializerContext : JsonSerializerContext
    {
        public static JsonPresenceArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonPresence>))]
    public partial class IReadOnlyCollectionOfJsonPresenceSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonPresenceSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
