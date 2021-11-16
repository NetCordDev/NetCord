namespace NetCord
{
    public class StageGuildChannel : Channel, IGuildChannel, IVoiceChannel
    {
        public int Bitrate => (int)_jsonEntity.Bitrate;
        public DiscordId? CategoryId => _jsonEntity.ParentId;
        public string? Topic => _jsonEntity.Topic;
        public string RtcRegion => _jsonEntity.RtcRegion;
        public VideoQualityMode VideoQualityMode => (VideoQualityMode)_jsonEntity.VideoQualityMode;

        public string Name => _jsonEntity.Name;

        public int Position => (int)_jsonEntity.Position;

        public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }

        public Guild Guild { get; }

        internal StageGuildChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
            PermissionOverwrites = jsonEntity.PermissionOverwrites.SelectOrEmpty(p => new PermissionOverwrite(p, client));
            if (client.TryGetGuild(jsonEntity.GuildId, out Guild guild))
                Guild = guild;
        }
    }
}
