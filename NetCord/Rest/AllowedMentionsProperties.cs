using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(AllowedMentionsConverter))]
public partial class AllowedMentionsProperties
{
    /// <summary>
    /// Allows <see langword="@everyone"/> and <see langword="@here"/>.
    /// </summary>
    public bool Everyone { get; set; } = true;

    /// <summary>
    /// Allowed roles, <see langword="null"/> for all.
    /// </summary>
    public IEnumerable<ulong>? AllowedRoles { get; set; }

    /// <summary>
    /// Allowed users, <see langword="null"/> for all.
    /// </summary>
    public IEnumerable<ulong>? AllowedUsers { get; set; }

    /// <summary>
    /// Allows reply mention.
    /// </summary>
    public bool ReplyMention { get; set; }

    public static AllowedMentionsProperties All => new()
    {
        ReplyMention = true,
    };

    public static AllowedMentionsProperties None
    {
        get
        {
            AllowedMentionsProperties result = new()
            {
                Everyone = false,
            };
            result.AllowedRoles = result.AllowedUsers = Enumerable.Empty<ulong>();
            return result;
        }
    }

    public partial class AllowedMentionsConverter : JsonConverter<AllowedMentionsProperties>
    {
        private static readonly JsonEncodedText _roles = JsonEncodedText.Encode("roles");
        private static readonly JsonEncodedText _users = JsonEncodedText.Encode("users");
        private static readonly JsonEncodedText _everyone = JsonEncodedText.Encode("everyone");
        private static readonly JsonEncodedText _parse = JsonEncodedText.Encode("parse");
        private static readonly JsonEncodedText _repliedUser = JsonEncodedText.Encode("replied_user");

        public override AllowedMentionsProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, AllowedMentionsProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var parse = new JsonEncodedText[3];
            int index = 0;
            var allowedRoles = value.AllowedRoles;
            if (allowedRoles is null)
                parse[index++] = _roles;
            else
            {
                writer.WritePropertyName(_roles);
                JsonSerializer.Serialize(writer, allowedRoles, Serialization.Default.IEnumerableUInt64);
            }

            var allowedUsers = value.AllowedUsers;
            if (allowedUsers is null)
                parse[index++] = _users;
            else
            {
                writer.WritePropertyName(_users);
                JsonSerializer.Serialize(writer, allowedUsers, Serialization.Default.IEnumerableUInt64);
            }

            if (value.Everyone)
                parse[index++] = _everyone;

            writer.WritePropertyName(_parse);

            writer.WriteStartArray();
            for (int i = 0; i < index; i++)
                writer.WriteStringValue(parse[i]);
            writer.WriteEndArray();

            if (value.ReplyMention)
                writer.WriteBoolean(_repliedUser, true);

            writer.WriteEndObject();
        }
    }
}
