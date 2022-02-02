﻿using System.Collections.Immutable;

namespace NetCord
{
    public class GuildWidget : Entity
    {
        private readonly JsonModels.JsonGuildWidget _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id;

        public string Name => _jsonEntity.Name;

        public string? InstantInvite => _jsonEntity.InstantInvite;

        public ImmutableDictionary<DiscordId, GuildWidgetChannel> Channels { get; }

        public ImmutableDictionary<DiscordId, User> Users { get; }

        public int PresenceCount => _jsonEntity.PresenceCount;

        internal GuildWidget(JsonModels.JsonGuildWidget jsonEntity, RestClient client)
        {
            _jsonEntity = jsonEntity;
            Channels = _jsonEntity.Channels.ToImmutableDictionary(c => c.Id, c => new GuildWidgetChannel(c));
            Users = _jsonEntity.Users.ToImmutableDictionary(u => u.Id, u => new User(u, client));
        }
    }
}