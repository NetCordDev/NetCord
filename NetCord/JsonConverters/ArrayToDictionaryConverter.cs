using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.JsonConverters;

internal partial class JsonGuildRoleArrayToDictionaryConverter : JsonConverter<Dictionary<ulong, JsonGuildRole>>
{
    public override Dictionary<ulong, JsonGuildRole>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildRole.JsonGuildRoleArraySerializerContext.WithOptions.JsonGuildRoleArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ulong, JsonGuildRole> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonGuildRoleSerializerContext.WithOptions.IReadOnlyCollectionJsonGuildRole);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonGuildRole>))]
    public partial class IReadOnlyCollectionOfJsonGuildRoleSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonGuildRoleSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonChannelArrayToDictionaryConverter : JsonConverter<Dictionary<ulong, JsonChannel>>
{
    public override Dictionary<ulong, JsonChannel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonChannel.JsonChannelArraySerializerContext.WithOptions.JsonChannelArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ulong, JsonChannel> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonChannelSerializerContext.WithOptions.IReadOnlyCollectionJsonChannel);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonChannel>))]
    public partial class IReadOnlyCollectionOfJsonChannelSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonChannelSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonStageInstanceArrayToDictionaryConverter : JsonConverter<Dictionary<ulong, JsonStageInstance>>
{
    public override Dictionary<ulong, JsonStageInstance>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonStageInstanceArraySerializerContext.WithOptions.JsonStageInstanceArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ulong, JsonStageInstance> value, JsonSerializerOptions options)
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

internal partial class JsonGuildScheduledEventArrayToDictionaryConverter : JsonConverter<Dictionary<ulong, JsonGuildScheduledEvent>>
{
    public override Dictionary<ulong, JsonGuildScheduledEvent>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildScheduledEvent.JsonGuildScheduledEventArraySerializerContext.WithOptions.JsonGuildScheduledEventArray).ToDictionary(v => v.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ulong, JsonGuildScheduledEvent> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonGuildScheduledEventSerializerContext.WithOptions.IReadOnlyCollectionJsonGuildScheduledEvent);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonGuildScheduledEvent>))]
    public partial class IReadOnlyCollectionOfJsonGuildScheduledEventSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonGuildScheduledEventSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonVoiceStateArrayToDictionaryConverter : JsonConverter<Dictionary<ulong, JsonVoiceState>>
{
    public override Dictionary<ulong, JsonVoiceState>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonVoiceStateArraySerializerContext.WithOptions.JsonVoiceStateArray).ToDictionary(v => v.UserId);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ulong, JsonVoiceState> value, JsonSerializerOptions options)
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

internal partial class JsonGuildUserArrayToDictionaryConverter : JsonConverter<Dictionary<ulong, JsonGuildUser>>
{
    public override Dictionary<ulong, JsonGuildUser>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).DistinctBy(u => u.User.Id).ToDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ulong, JsonGuildUser> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Values, IReadOnlyCollectionOfJsonGuildUserSerializerContext.WithOptions.IReadOnlyCollectionJsonGuildUser);
    }

    [JsonSerializable(typeof(IReadOnlyCollection<JsonGuildUser>))]
    public partial class IReadOnlyCollectionOfJsonGuildUserSerializerContext : JsonSerializerContext
    {
        public static IReadOnlyCollectionOfJsonGuildUserSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

internal partial class JsonPresenceArrayToDictionaryConverter : JsonConverter<Dictionary<ulong, JsonPresence>>
{
    public override Dictionary<ulong, JsonPresence>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(JsonPresenceArraySerializerContext.WithOptions.JsonPresenceArray).ToDictionary(v => v.User.Id);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<ulong, JsonPresence> value, JsonSerializerOptions options)
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
