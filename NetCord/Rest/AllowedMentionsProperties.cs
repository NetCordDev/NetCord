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
    public IEnumerable<Snowflake>? TheOnlyAllowedRoles { get; set; }
    /// <summary>
    /// <see langword="null"/> for all
    /// </summary>
    public IEnumerable<Snowflake>? TheOnlyAllowedUsers { get; set; }
    public bool ReplyMention { get; set; }

    public static AllowedMentionsProperties All => new()
    {
        ReplyMention = true
    };

    public static AllowedMentionsProperties None => new()
    {
        TheOnlyAllowedRoles = Enumerable.Empty<Snowflake>(),
        TheOnlyAllowedUsers = Enumerable.Empty<Snowflake>(),
        Everyone = false,
    };

    internal partial class AllowedMentionsConverter : JsonConverter<AllowedMentionsProperties>
    {
        public override AllowedMentionsProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, AllowedMentionsProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            List<string> list = new(3);
            if (value.TheOnlyAllowedRoles == null)
                list.Add("roles");
            else
            {
                writer.WritePropertyName("roles");
                JsonSerializer.Serialize(writer, value.TheOnlyAllowedRoles, IEnumerableOfSnowflake.WithOptions.IEnumerableSnowflake);
            }
            if (value.TheOnlyAllowedUsers == null)
                list.Add("users");
            else
            {
                writer.WritePropertyName("users");
                JsonSerializer.Serialize(writer, value.TheOnlyAllowedUsers, IEnumerableOfSnowflake.WithOptions.IEnumerableSnowflake);
            }
            if (value.Everyone)
                list.Add("everyone");
            writer.WritePropertyName("parse");
            JsonSerializer.Serialize(writer, list, ListOfString.WithOptions.ListString);

            if (value.ReplyMention == true)
            {
                writer.WritePropertyName("replied_user");
                writer.WriteBooleanValue(value.ReplyMention);
            }
            writer.WriteEndObject();
        }

        [JsonSerializable(typeof(IEnumerable<Snowflake>))]
        public partial class IEnumerableOfSnowflake : JsonSerializerContext
        {
            public static IEnumerableOfSnowflake WithOptions { get; } = new(new(ToObjectExtensions._options));
        }

        [JsonSerializable(typeof(List<string>))]
        public partial class ListOfString : JsonSerializerContext
        {
            public static ListOfString WithOptions { get; } = new(new(ToObjectExtensions._options));
        }
    }

    [JsonSerializable(typeof(AllowedMentionsProperties))]
    public partial class AllowedMentionsPropertiesSerializerContext : JsonSerializerContext
    {
        public static AllowedMentionsPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
