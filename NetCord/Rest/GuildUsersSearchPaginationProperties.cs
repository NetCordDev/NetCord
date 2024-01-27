using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(GuildUsersSearchPaginationPropertiesConverter))]
public partial record GuildUsersSearchPaginationProperties : PaginationProperties<GuildUsersSearchTimestamp>, IPaginationProperties<GuildUsersSearchTimestamp, GuildUsersSearchPaginationProperties>
{
    public IEnumerable<IGuildUsersSearchQuery>? OrQuery { get; set; }

    public IEnumerable<IGuildUsersSearchQuery>? AndQuery { get; set; }

    static GuildUsersSearchPaginationProperties IPaginationProperties<GuildUsersSearchTimestamp, GuildUsersSearchPaginationProperties>.Create() => new();
    static GuildUsersSearchPaginationProperties IPaginationProperties<GuildUsersSearchTimestamp, GuildUsersSearchPaginationProperties>.Create(GuildUsersSearchPaginationProperties properties) => new(properties);

    public class GuildUsersSearchPaginationPropertiesConverter : JsonConverter<GuildUsersSearchPaginationProperties>
    {
        private static readonly JsonEncodedText _orQuery = JsonEncodedText.Encode("or_query");
        private static readonly JsonEncodedText _andQuery = JsonEncodedText.Encode("and_query");
        private static readonly JsonEncodedText _limit = JsonEncodedText.Encode("limit");
        private static readonly JsonEncodedText _after = JsonEncodedText.Encode("after");
        private static readonly JsonEncodedText _before = JsonEncodedText.Encode("before");

        public override GuildUsersSearchPaginationProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, GuildUsersSearchPaginationProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var orQuery = value.OrQuery;
            if (orQuery is not null)
            {
                writer.WriteStartObject(_orQuery);

                foreach (var query in orQuery)
                    query.Serialize(writer);

                writer.WriteEndObject();
            }

            var andQuery = value.AndQuery;
            if (andQuery is not null)
            {
                writer.WriteStartObject(_andQuery);

                foreach (var query in andQuery)
                    query.Serialize(writer);

                writer.WriteEndObject();
            }

            writer.WriteNumber(_limit, value.Limit.GetValueOrDefault());

            var direction = value.Direction.GetValueOrDefault();

            var from = value.From;
            if (from.HasValue)
            {
                writer.WritePropertyName(direction switch
                {
                    // Reversed for a reason.
                    PaginationDirection.Before => _after,
                    PaginationDirection.After => _before,
                    _ => throw new ArgumentException($"The value of '{nameof(direction)}' is invalid."),
                });

                JsonSerializer.Serialize(writer, from, Serialization.Default.GuildUsersSearchTimestamp);
            }

            writer.WriteEndObject();
        }
    }
}
