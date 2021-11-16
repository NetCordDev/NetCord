namespace NetCord
{
    public interface ISocketInteraction
    {
        public DiscordId Id { get; }
        public DiscordId ApplicationId { get; }
        public DiscordId GuildId { get; }
        public DiscordId ChannelId { get; }
        public User User { get; }
        public string Token { get; }
    }
}