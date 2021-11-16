//namespace NetCord;

//public class GuildMessage : Message
//{
//    public DiscordId GuildId { get; }

//    public Guild Guild { get; }

//    public override GuildUser Author { get; }



//    internal GuildMessage(JsonModels.JsonMessage jsonEntity, BotClient client) : base(jsonEntity, client)
//    {
//        GuildId = jsonEntity.GuildId ?? jsonEntity.MessageReference.GuildId;
//        Guild = client.GetGuild(GuildId);
//        Author = new(jsonEntity.Author, jsonEntity.Member, client, Guild);
//        if (Guild.TryGetChannel(ChannelId, out IGuildChannel channel))
//            Channel = (TextChannel)channel;
//    }
//}