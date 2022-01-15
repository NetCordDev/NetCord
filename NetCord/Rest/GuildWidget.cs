namespace NetCord
{
    public class GuildWidget : Entity
    {
        private readonly JsonModels.JsonGuildWidget _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id;

        public string Name => _jsonEntity.Name;

        public string? InstantInvite => _jsonEntity.InstantInvite;

        public IReadOnlyDictionary<DiscordId, GuildWidgetChannel> Channels { get; }

        public IReadOnlyDictionary<DiscordId, User> Users { get; }

        public int PresenceCount => _jsonEntity.PresenceCount;

        internal GuildWidget(JsonModels.JsonGuildWidget jsonEntity, RestClient client)
        {
            _jsonEntity = jsonEntity;
            Channels = _jsonEntity.Channels.ToDictionary(c => c.Id, c => new GuildWidgetChannel(c));
            Users = _jsonEntity.Users.ToDictionary(u => u.Id, u => new User(u, client));
        }
    }
}