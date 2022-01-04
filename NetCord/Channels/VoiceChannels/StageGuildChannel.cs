namespace NetCord
{
    public class StageGuildChannel : Channel, IGuildChannel, IVoiceChannel
    {
        public int Bitrate => (int)_jsonEntity.Bitrate!;
        public DiscordId? CategoryId => _jsonEntity.ParentId;
        public string? Topic => _jsonEntity.Topic;
        public string RtcRegion => _jsonEntity.RtcRegion;
        public VideoQualityMode VideoQualityMode => VideoQualityMode.None;

        public string Name => _jsonEntity.Name!;

        public int Position => (int)_jsonEntity.Position!;

        public IReadOnlyDictionary<DiscordId, PermissionOverwrite> PermissionOverwrites { get; }

        internal StageGuildChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {
            PermissionOverwrites = jsonEntity.PermissionOverwrites.ToImmutableDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
        }
    }
}
