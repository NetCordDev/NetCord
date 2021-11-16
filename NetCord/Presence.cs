namespace NetCord
{
    public class Presence
    {
        private readonly JsonModels.JsonPresence _jsonEntity;

        public User User { get; }
        public DiscordId GuildId => _jsonEntity.GuildId;
        public UserStatus Status => _jsonEntity.Status;
        public IEnumerable<UserActivity> Activities { get; }
        public IReadOnlyDictionary<Platform, UserStatus> Platform => _jsonEntity.Platform;

        internal Presence(JsonModels.JsonPresence jsonEntity, BotClient client)
        {
            _jsonEntity = jsonEntity;
            Activities = jsonEntity.Activities.SelectOrEmpty(a => new UserActivity(a, client));
        }
    }
}