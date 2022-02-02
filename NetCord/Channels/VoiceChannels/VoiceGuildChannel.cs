﻿namespace NetCord
{
    public class VoiceGuildChannel : Channel, IVoiceGuildChannel
    {
        public int Bitrate => (int)_jsonEntity.Bitrate!;
        public DiscordId? CategoryId => _jsonEntity.ParentId;
        public int UserLimit => (int)_jsonEntity.UserLimit!; //
        public string RtcRegion => _jsonEntity.RtcRegion;
        public VideoQualityMode VideoQualityMode
            => _jsonEntity.VideoQualityMode != null ? (VideoQualityMode)_jsonEntity.VideoQualityMode : VideoQualityMode.Auto;

        public string Name => _jsonEntity.Name!;

        public int Position => (int)_jsonEntity.Position!;

        public IReadOnlyDictionary<DiscordId, PermissionOverwrite> PermissionOverwrites { get; }

        internal VoiceGuildChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {
            PermissionOverwrites = jsonEntity.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
        }

        public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestOptions? options = null) => (IGuildChannel)await _client.Guild.Channel.ModifyAsync(Id, action, options).ConfigureAwait(false);
    }
}
