namespace NetCord
{
    public class NewsGuildChannel : TextGuildChannel
    {
        internal NewsGuildChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {

        }
    }
}
