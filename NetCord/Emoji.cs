namespace NetCord
{
    public class Emoji : ClientEntity
    {
        private readonly JsonModels.JsonEmoji _jsonEntity;

        public override DiscordId Id
        {
            get
            {
                if (!_jsonEntity.Id.HasValue)
                    throw new InvalidOperationException("This emoji has no id");
                return _jsonEntity.Id.GetValueOrDefault();
            }
        }

        public bool IsStandard => !_jsonEntity.Id.HasValue;

        public string? Name => _jsonEntity.Name;

        public IReadOnlyDictionary<DiscordId, GuildRole> AllowedRoles { get; }

        public User? Creator { get; }

        public bool? RequireColons => _jsonEntity.RequireColons;

        public bool? Managed => _jsonEntity.Managed;

        public bool? Animated => _jsonEntity.Animated;

        public bool? Available => _jsonEntity.Available;

        internal Emoji(JsonModels.JsonEmoji jsonEntity, RestClient client) : base(client)
        {
            _jsonEntity = jsonEntity;
            if (jsonEntity.Creator != null)
                Creator = new(jsonEntity.Creator, client);
            AllowedRoles = jsonEntity.AllowedRoles.ToImmutableDictionaryOrEmpty(r => r.Id, r => new GuildRole(r, client));
        }
    }
}
