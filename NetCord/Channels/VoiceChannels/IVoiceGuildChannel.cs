namespace NetCord
{
    internal interface IVoiceGuildChannel : IGuildChannel
    {
        public int Bitrate { get; }
        public string RtcRegion { get; }
        public VideoQualityMode VideoQualityMode { get; }
    }
}
