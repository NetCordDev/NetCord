namespace NetCord
{
    internal interface IVoiceChannel
    {
        public int Bitrate { get; }
        public string RtcRegion { get; }
        public VideoQualityMode VideoQualityMode { get; }
    }
}
