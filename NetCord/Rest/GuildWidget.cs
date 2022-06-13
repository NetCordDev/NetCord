using System.Collections.Immutable;

namespace NetCord
{
    public class GuildWidget : Entity
    {
        private readonly JsonModels.JsonGuildWidget _jsonModel;

        public override Snowflake Id => _jsonModel.Id;

        public string Name => _jsonModel.Name;

        public string? InstantInvite => _jsonModel.InstantInvite;

        public ImmutableDictionary<Snowflake, GuildWidgetChannel> Channels { get; }

        public ImmutableDictionary<Snowflake, User> Users { get; }

        public int PresenceCount => _jsonModel.PresenceCount;

        public GuildWidget(JsonModels.JsonGuildWidget jsonModel, RestClient client)
        {
            _jsonModel = jsonModel;
            Channels = _jsonModel.Channels.ToImmutableDictionary(c => c.Id, c => new GuildWidgetChannel(c));
            Users = _jsonModel.Users.ToImmutableDictionary(u => u.Id, u => new User(u, client));
        }
    }
}