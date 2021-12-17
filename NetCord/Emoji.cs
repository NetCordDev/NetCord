namespace NetCord
{
    public class Emoji : ClientEntity
    {
        private readonly JsonModels.JsonEmoji _jsonEntity;

#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        public override DiscordId? Id => _jsonEntity.Id;
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

        public string? Name => _jsonEntity.Name;

        public IReadOnlyDictionary<DiscordId, Role> AllowedRoles { get; }

        public User? Creator { get; }

        public bool? RequireColons => _jsonEntity.RequireColons;

        public bool? Managed => _jsonEntity.Managed;

        public bool? Animated => _jsonEntity.Animated;

        public bool? Available => _jsonEntity.Available;

        internal Emoji(JsonModels.JsonEmoji jsonEntity, BotClient client) : base(client)
        {
            _jsonEntity = jsonEntity;
            if (jsonEntity.Creator != null)
                Creator = new(jsonEntity.Creator, client);
            AllowedRoles = jsonEntity.AllowedRoles.ToDictionaryOrEmpty(r => r.Id, r => new Role(r, client));
        }
    }
}
