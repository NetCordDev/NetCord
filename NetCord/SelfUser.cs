namespace NetCord
{
    public class SelfUser : User
    {
        //public async Task<SocketSelfUser> ModifyAsync(Action<Properties> func, DiscordSocketClient client)
        //{
        //    Properties properties = new();
        //    func.Invoke(properties);
        //    var message = JsonSerializer.Serialize(properties);
        //    var result = await CDN.SendAsync(HttpMethod.Patch, message, $"/users/@me", client).ConfigureAwait(false);
        //    return result.ToObject<SocketSelfUser>();
        //}

        //[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
        //public class Properties
        //{
        //    [JsonPropertyName("username")]
        //    public string Username { get; set; }
        //    [JsonPropertyName("avatar")]
        //    public Avatar Avatar { get; set; }
        //}

        //[JsonConverter(typeof(JsonConverters.AvatarConverter))]
        //public class Avatar
        //{
        //    public ContentType ContentType { get; }
        //    public string AvatarBase64 { get; }

        //    public Avatar(ContentType contentType, string avatarBase64)
        //    {
        //        ContentType = contentType;
        //        AvatarBase64 = avatarBase64;
        //    }
        //}

        internal SelfUser(JsonModels.JsonUser jsonEntity, BotClient client) : base(jsonEntity, client)
        {

        }
    }
}