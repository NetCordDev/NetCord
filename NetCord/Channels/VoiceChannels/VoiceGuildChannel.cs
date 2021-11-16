namespace NetCord
{
    public class VoiceGuildChannel : Channel, IGuildChannel, IVoiceChannel
    {
        public int Bitrate => (int)_jsonEntity.Bitrate;
        public DiscordId? CategoryId => _jsonEntity.ParentId;
        public int UserLimit => (int)_jsonEntity.UserLimit; //
        public string RtcRegion => _jsonEntity.RtcRegion;
        public VideoQualityMode VideoQualityMode
            => _jsonEntity.VideoQualityMode == null ? (VideoQualityMode)_jsonEntity.VideoQualityMode : VideoQualityMode.Auto;

        public string Name => _jsonEntity.Name;

        public int Position => (int)_jsonEntity.Position;

        public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }

        public Guild Guild { get; }

        internal VoiceGuildChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
        }
    }
}
