using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    public class SelfUser : User
    {
        public async Task<SelfUser> ModifyAsync(Action<Properties> func)
        {
            Properties properties = new();
            func.Invoke(properties);
            var message = JsonSerializer.Serialize(properties);
            var result = (await CDN.SendAsync(HttpMethod.Patch, message, $"/users/@me", _client).ConfigureAwait(false))!;
            return new(result.ToObject<JsonModels.JsonUser>(), _client);
        }

        public class Properties
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("username")]
            public string? Username { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("avatar")]
            public Avatar? Avatar { get; set; }
        }

        [JsonConverter(typeof(AvatarConverter))]
        public class Avatar
        {
            public ContentType ContentType { get; }
            public string AvatarBase64 { get; }

            public Avatar(ContentType contentType, string avatarBase64)
            {
                ContentType = contentType;
                AvatarBase64 = avatarBase64;
            }

            private class AvatarConverter : JsonConverter<Avatar>
            {
                public override Avatar Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                    throw new NotImplementedException();
                }

                public override void Write(Utf8JsonWriter writer, Avatar value, JsonSerializerOptions options)
                {
                    writer.WriteStringValue("data:" + value.ContentType.MediaType + ";base64," + value.AvatarBase64);
                }
            }
        }

        internal SelfUser(JsonModels.JsonUser jsonEntity, BotClient client) : base(jsonEntity, client)
        {
        }
    }
}