using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(AllowedMentionsConverter))]
public partial class AllowedMentionsProperties
{
    /// <summary>
    /// <see langword="@everyone"/> and <see langword="@here"/>
    /// </summary>
    public bool Everyone { get; set; } = true;

    /// <summary>
    /// <see langword="null"/> for all
    /// </summary>
    public IEnumerable<ulong>? AllowedRoles { get; set; }

    /// <summary>
    /// <see langword="null"/> for all
    /// </summary>
    public IEnumerable<ulong>? AllowedUsers { get; set; }

    public bool ReplyMention { get; set; }

    public static AllowedMentionsProperties All => new()
    {
        ReplyMention = true,
    };

    public static AllowedMentionsProperties None => new()
    {
        Everyone = false,
        AllowedRoles = Enumerable.Empty<ulong>(),
        AllowedUsers = Enumerable.Empty<ulong>(),
    };

    internal partial class AllowedMentionsConverter : JsonConverter<AllowedMentionsProperties>
    {
        public override AllowedMentionsProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, AllowedMentionsProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            List<string> list = new(3);
            if (value.AllowedRoles == null)
                list.Add("roles");
            else
            {
                writer.WritePropertyName("roles");
                JsonSerializer.Serialize(writer, value.AllowedRoles, IEnumerableOfUInt64.WithOptions.IEnumerableUInt64);
            }
            if (value.AllowedUsers == null)
                list.Add("users");
            else
            {
                writer.WritePropertyName("users");
                JsonSerializer.Serialize(writer, value.AllowedUsers, IEnumerableOfUInt64.WithOptions.IEnumerableUInt64);
            }
            if (value.Everyone)
                list.Add("everyone");
            writer.WritePropertyName("parse");
            JsonSerializer.Serialize(writer, list, ListOfString.WithOptions.ListString);

            if (value.ReplyMention == true)
            {
                writer.WritePropertyName("replied_user");
                writer.WriteBooleanValue(true);
            }
            writer.WriteEndObject();
        }

        [JsonSerializable(typeof(IEnumerable<ulong>))]
        public partial class IEnumerableOfUInt64 : JsonSerializerContext
        {
            public static IEnumerableOfUInt64 WithOptions { get; } = new(Serialization.Options);
        }

        [JsonSerializable(typeof(List<string>))]
        public partial class ListOfString : JsonSerializerContext
        {
            public static ListOfString WithOptions { get; } = new(Serialization.Options);
        }
    }

    [JsonSerializable(typeof(AllowedMentionsProperties))]
    public partial class AllowedMentionsPropertiesSerializerContext : JsonSerializerContext
    {
        public static AllowedMentionsPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
