namespace NetCord;

public partial class RestClient
{
    public partial class GuildModule
    {
        private readonly BotClient _client;

        public UserModule User { get; }

        internal GuildModule(BotClient client)
        {
            _client = client;
            User = new(client);
        }
    }
}